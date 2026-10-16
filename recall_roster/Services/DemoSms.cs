using Twilio.Clients;
using Twilio.Rest.Verify.V2.Service;

namespace recall_roster.Services;

public interface IDemoSms
{
    Task StartVerification(string phone);
    Task<bool> CheckVerification(string phone, string code);
    Task Send(string phone, string body);
}

public sealed class DemoSms(IConfiguration configuration, ISmsSender sender) : IDemoSms
{
    private string Required(string key) => configuration[key] is { Length: > 0 } value
        ? value : throw new InvalidOperationException("Demo SMS configuration is incomplete.");
    private TwilioRestClient Client() => new(Required("Twilio:AccountSid"), Required("Twilio:AuthToken"));
    public async Task StartVerification(string phone) => await VerificationResource.CreateAsync(
        pathServiceSid: Required("Twilio:VerifyServiceSid"), to: phone, channel: "sms", client: Client());
    public async Task<bool> CheckVerification(string phone, string code)
    {
        var result = await VerificationCheckResource.CreateAsync(pathServiceSid: Required("Twilio:VerifyServiceSid"),
            to: phone, code: code, client: Client());
        return result.Status == "approved";
    }
    public Task Send(string phone, string body) => sender.SendAsync(phone, body);
}
