using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using recall_roster.Controllers;
using recall_roster.Services;
using Xunit;

namespace recall_roster.Tests;

public class DemoTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "recall-demo-test-" + Guid.NewGuid());
    private IConfiguration Config(bool enabled = true, int limit = 20) => new ConfigurationBuilder().AddInMemoryCollection(
        new Dictionary<string, string?> { ["Demo:StorePath"] = Path.Combine(_directory, "state.json"), ["Demo:Enabled"] = enabled.ToString(), ["Demo:DailyLimit"] = limit.ToString() }).Build();
    private const string Phone = "+12025550123";
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }

    [Fact]
    public async Task TwoSendLimitIsAtomicAndPersistsAcrossStoresAndSessions()
    {
        var store = new DemoStore(Config());
        var first = store.Start(Phone);
        var second = store.Start(Phone);
        store.Verify(first); store.Verify(second);
        var results = await Task.WhenAll(Enumerable.Range(0, 12).Select(index => Task.Run(() =>
        {
            try { new DemoStore(Config()).ReserveSend(index % 2 == 0 ? first : second); return true; }
            catch (DemoError error) { Assert.Equal(429, error.Status); return false; }
        })));
        Assert.Equal(2, results.Count(x => x));
        Assert.Equal(429, Assert.Throws<DemoError>(() => new DemoStore(Config()).Start(Phone)).Status);
    }

    [Fact]
    public void CannotSendWithoutVerificationOrGuessUnlimitedCodes()
    {
        var store = new DemoStore(Config());
        var token = store.Start(Phone);
        Assert.Equal(403, Assert.Throws<DemoError>(() => store.ReserveSend(token)).Status);
        for (var i = 0; i < 5; i++) Assert.Equal(Phone, store.ReserveCheck(token));
        Assert.Equal(429, Assert.Throws<DemoError>(() => store.ReserveCheck(token)).Status);
        Assert.Equal(401, Assert.Throws<DemoError>(() => store.ReserveSend(new string('a', 64))).Status);
    }

    [Fact]
    public void GlobalAndPhoneVerificationLimitsArePersistent()
    {
        var store = new DemoStore(Config(limit: 4));
        for (var i = 0; i < 3; i++) store.Start(Phone);
        Assert.Equal(429, Assert.Throws<DemoError>(() => store.Start(Phone)).Status);
        store.Start("+12025550124");
        Assert.Equal(429, Assert.Throws<DemoError>(() => new DemoStore(Config(limit: 4)).Start("+12025550125")).Status);
    }

    [Fact]
    public void CorruptLedgerDoesNotResetQuotas()
    {
        var store = new DemoStore(Config());
        store.Start(Phone);
        File.WriteAllText(Path.Combine(_directory, "state.json"), "corrupt");
        Assert.Throws<System.Text.Json.JsonException>(() => store.Start(Phone));
    }

    private sealed class FakeSms : IDemoSms
    {
        public bool Approved;
        public bool FailSend;
        public int Starts;
        public List<string> Destinations = new();
        public Task StartVerification(string phone) { Starts++; return Task.CompletedTask; }
        public Task<bool> CheckVerification(string phone, string code) => Task.FromResult(Approved);
        public Task Send(string phone, string body)
        {
            Destinations.Add(phone);
            if (FailSend) throw new Twilio.Exceptions.ApiException("Provider failure");
            return Task.CompletedTask;
        }
    }
    private static DemoController Controller(DemoStore store, FakeSms sms, IConfiguration config, string token = "")
    {
        var controller = new DemoController(store, sms, config) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
        controller.Request.Headers["X-Demo-Session"] = token;
        return controller;
    }

    [Fact]
    public async Task DisabledDemoConsentAndRegionValidationNeverCallProvider()
    {
        var sms = new FakeSms();
        var store = new DemoStore(Config());
        Assert.IsType<NotFoundResult>(await Controller(store, sms, Config(false)).Start(new(Phone, true)));
        Assert.Equal(400, Assert.IsType<ObjectResult>(await Controller(store, sms, Config()).Start(new(Phone, false))).StatusCode);
        Assert.Equal(400, Assert.IsType<ObjectResult>(await Controller(store, sms, Config()).Start(new("+442071234567", true))).StatusCode);
        Assert.Equal(0, sms.Starts);
    }

    [Fact]
    public async Task OnlyApprovedPhoneCanSendAndProviderFailuresConsumeQuota()
    {
        var config = Config(); var store = new DemoStore(config); var sms = new FakeSms();
        var token = store.Start(Phone); var controller = Controller(store, sms, config, token);
        Assert.Equal(400, Assert.IsType<ObjectResult>(await controller.Check(new("123456"))).StatusCode);
        Assert.Equal(403, Assert.IsType<ObjectResult>(await controller.Send(new("recall"))).StatusCode);
        sms.Approved = true;
        Assert.IsType<OkObjectResult>(await controller.Check(new("123456")));
        Assert.Equal(400, Assert.IsType<ObjectResult>(await controller.Send(new("arbitrary content"))).StatusCode);
        sms.FailSend = true;
        Assert.Equal(502, Assert.IsType<ObjectResult>(await controller.Send(new("recall"))).StatusCode);
        sms.FailSend = false;
        Assert.IsType<OkObjectResult>(await controller.Send(new("reminder")));
        Assert.Equal(429, Assert.IsType<ObjectResult>(await controller.Send(new("recall"))).StatusCode);
        Assert.Equal(new[] { Phone, Phone }, sms.Destinations);
    }
}
