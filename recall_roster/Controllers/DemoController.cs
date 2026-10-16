using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Services;
using System.Text.RegularExpressions;

namespace recall_roster.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/demo")]
[RequestSizeLimit(2048)]
public sealed class DemoController(DemoStore store, IDemoSms sms, IConfiguration configuration) : ControllerBase
{
    public record PhoneRequest(string Phone, bool Consent);
    public record CodeRequest(string Code);
    public record SendRequest(string Template);
    private string Token => Request.Headers["X-Demo-Session"].ToString();
    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        if (!configuration.GetValue<bool>("Demo:Enabled")) return NotFound();
        Response.Headers.CacheControl = "no-store";
        try { return await action(); }
        catch (DemoError error) { return StatusCode(error.Status, new { message = error.Message }); }
        catch (Twilio.Exceptions.ApiException) { return StatusCode(502, new { message = "The SMS provider could not complete this request. Any reserved attempt still counts; check your phone before trying again." }); }
    }

    [HttpGet("session")]
    public Task<IActionResult> Status() => Run(() => Task.FromResult<IActionResult>(Ok(store.Status(Token))));

    [HttpPost("verify/start")]
    public Task<IActionResult> Start(PhoneRequest request) => Run(async () =>
    {
        var phone = (request.Phone ?? "").Trim();
        if (!request.Consent) throw new DemoError(400, "Confirm that this is your phone and you agree to receive the demo texts.");
        if (!Regex.IsMatch(phone, @"^\+[1-9][0-9]{7,14}$")) throw new DemoError(400, "Enter your number with its country code, for example +12025550123.");
        var prefixes = (configuration["Demo:AllowedPrefixes"] ?? "+1").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (!prefixes.Any(prefix => phone.StartsWith(prefix, StringComparison.Ordinal)))
            throw new DemoError(400, "The demo does not support this phone number's country code.");
        var token = store.Start(phone);
        await sms.StartVerification(phone);
        return Ok(new { token, message = "Verification code sent. It expires in 10 minutes." });
    });

    [HttpPost("verify/check")]
    public Task<IActionResult> Check(CodeRequest request) => Run(async () =>
    {
        if (!Regex.IsMatch(request.Code ?? "", @"^[0-9]{4,10}$")) throw new DemoError(400, "Enter the code from your text message.");
        var phone = store.ReserveCheck(Token);
        if (!await sms.CheckVerification(phone, request.Code!)) throw new DemoError(400, "That code is incorrect or expired.");
        return Ok(new { remaining = store.Verify(Token), verified = true });
    });

    [HttpPost("send")]
    public Task<IActionResult> Send(SendRequest request) => Run(async () =>
    {
        var body = request.Template switch
        {
            "recall" => "Recall Roster DEMO: A test recall has started. Please check in with your coordinator. No action is required. Reply STOP to opt out.",
            "reminder" => "Recall Roster DEMO: This is a test recall reminder. No action is required. Thanks for trying the demo! Reply STOP to opt out.",
            _ => throw new DemoError(400, "Choose a demo message.")
        };
        var send = store.ReserveSend(Token);
        await sms.Send(send.Phone, body);
        return Ok(new { remaining = send.Remaining, message = "Submitted to the SMS provider. Check your phone; delivery may take a moment." });
    });
}

