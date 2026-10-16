using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace recall_roster.Services;

public class DemoError(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}

public sealed class DemoSession
{
    public string Phone { get; set; } = "";
    public DateTimeOffset Expires { get; set; }
    public bool Verified { get; set; }
    public int Checks { get; set; }
}

public sealed class DemoLedger
{
    public Dictionary<string, DemoSession> Sessions { get; set; } = new();
    public Dictionary<string, int> Sends { get; set; } = new();
    public string Day { get; set; } = "";
    public int VerificationsToday { get; set; }
    public int SendsToday { get; set; }
    public Dictionary<string, int> PhoneVerificationsToday { get; set; } = new();
}

// One deployment, one persistent disk. An exclusive lock protects the read/modify/write
// transaction across requests and processes; atomic replacement preserves the last committed ledger.
public sealed class DemoStore
{
    private readonly string _path;
    private readonly int _dailyLimit;
    public DemoStore(IConfiguration configuration)
    {
        _path = Path.GetFullPath(configuration["Demo:StorePath"] ?? "App_Data/demo-ledger.json");
        _dailyLimit = configuration.GetValue("Demo:DailyLimit", 20);
        if (_dailyLimit < 1 || _dailyLimit > 100) throw new InvalidOperationException("Demo:DailyLimit must be between 1 and 100.");
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private T Change<T>(Func<DemoLedger, T> change)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        FileStream? fileLock = null;
        for (var attempt = 0; fileLock == null; attempt++)
        {
            try { fileLock = new FileStream(_path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) when (attempt < 60) { Thread.Sleep(25); }
        }
        using (fileLock)
        {
            // Corrupt or unreadable state fails closed; it must never silently reset quotas.
            var state = File.Exists(_path)
                ? JsonSerializer.Deserialize<DemoLedger>(File.ReadAllText(_path)) ?? throw new IOException("Invalid demo ledger.")
                : new DemoLedger();
            var now = DateTimeOffset.UtcNow;
            foreach (var key in state.Sessions.Where(x => x.Value.Expires <= now).Select(x => x.Key).ToArray()) state.Sessions.Remove(key);
            var day = now.ToString("yyyy-MM-dd");
            if (state.Day != day)
            {
                state.Day = day;
                state.VerificationsToday = state.SendsToday = 0;
                state.PhoneVerificationsToday.Clear();
            }
            var result = change(state);
            using (var output = new FileStream(_path + ".tmp", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(output, state);
                output.Flush(true);
            }
            File.Move(_path + ".tmp", _path, true);
            return result;
        }
    }

    public string Start(string phone) => Change(state =>
    {
        if (state.Sends.GetValueOrDefault(Hash(phone)) >= 2) throw new DemoError(429, "This phone number has used its two demo sends.");
        if (state.VerificationsToday >= _dailyLimit || state.PhoneVerificationsToday.GetValueOrDefault(Hash(phone)) >= 3)
            throw new DemoError(429, "The verification limit has been reached. Please try another day.");
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        state.VerificationsToday++;
        state.PhoneVerificationsToday[Hash(phone)] = state.PhoneVerificationsToday.GetValueOrDefault(Hash(phone)) + 1;
        state.Sessions[Hash(token)] = new DemoSession { Phone = phone, Expires = DateTimeOffset.UtcNow.AddMinutes(10) };
        return token;
    });

    private static DemoSession Session(DemoLedger state, string token)
    {
        if (token.Length != 64 || !state.Sessions.TryGetValue(Hash(token), out var session))
            throw new DemoError(401, "Your demo session expired. Verify your phone again to continue.");
        return session;
    }

    public string ReserveCheck(string token) => Change(state =>
    {
        var session = Session(state, token);
        if (session.Verified) throw new DemoError(400, "This phone is already verified.");
        if (session.Checks >= 5) throw new DemoError(429, "Too many code attempts. Request a new code.");
        session.Checks++;
        return session.Phone;
    });

    public int Verify(string token) => Change(state =>
    {
        var session = Session(state, token);
        session.Verified = true;
        session.Expires = DateTimeOffset.UtcNow.AddMinutes(30);
        return Math.Max(0, 2 - state.Sends.GetValueOrDefault(Hash(session.Phone)));
    });

    public object Status(string token) => Change<object>(state =>
    {
        var session = Session(state, token);
        return new { verified = session.Verified, remaining = Math.Max(0, 2 - state.Sends.GetValueOrDefault(Hash(session.Phone))),
            phone = "••• ••• " + session.Phone[^4..], expiresAt = session.Expires };
    });

    // Reserve BEFORE calling Twilio. Failure/timeouts still consume an attempt, preventing
    // retries after an ambiguous response from exceeding two actual messages.
    public (string Phone, int Remaining) ReserveSend(string token) => Change(state =>
    {
        var session = Session(state, token);
        if (!session.Verified) throw new DemoError(403, "Verify your phone before sending.");
        var key = Hash(session.Phone);
        var used = state.Sends.GetValueOrDefault(key);
        if (used >= 2) throw new DemoError(429, "You have used both demo sends. Thanks for trying Recall Roster!");
        if (state.SendsToday >= _dailyLimit) throw new DemoError(429, "The demo's daily send limit has been reached. Please try another day.");
        state.Sends[key] = used + 1;
        state.SendsToday++;
        return (session.Phone, 1 - used);
    });
}
