using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Services;
using Twilio.AspNet.Core;
using Twilio.Security;
using Twilio.TwiML;

namespace recall_roster.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : TwilioController
{
    private readonly IMessageService _messages;
    private readonly IResponseService _responses;
    private readonly IConfiguration _configuration;
    public MessageController(IMessageService messages, IResponseService responses, IConfiguration configuration)
    {
        _messages = messages;
        _responses = responses;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("ReceiveMessage")]
    public async Task<IActionResult> ReceiveSms()
    {
        var token = _configuration["Twilio:AuthToken"];
        var signature = Request.Headers["X-Twilio-Signature"].ToString();
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(signature) || !Request.HasFormContentType)
            return StatusCode(StatusCodes.Status403Forbidden);
        var form = await Request.ReadFormAsync();
        // For a reverse proxy/ngrok, configure the exact public webhook URL (including query).
        // Do not trust client-supplied forwarded headers to reconstruct a signed URL.
        var url = _configuration["Twilio:WebhookUrl"] ?? Request.GetDisplayUrl();
        var parameters = form.ToDictionary(item => item.Key, item => item.Value.ToString());
        if (!new RequestValidator(token).Validate(url, parameters, signature))
            return StatusCode(StatusCodes.Status403Forbidden);
        var from = form["From"].ToString();
        var body = form["Body"].ToString();
        if (!TryReadIds(body, out var contactId, out var recallId))
            return TwiML(new MessagingResponse().Message("Reply with your contact ID followed by your recall ID."));
        try
        {
            _responses.AddResponse(from, body, recallId, contactId);
            return TwiML(new MessagingResponse().Message("Your response has been noted. Thank you."));
        }
        catch (ArgumentException)
        {
            return TwiML(new MessagingResponse().Message("Your response could not be matched to this recall. Check the IDs or contact your recall coordinator."));
        }
    }

    public static bool TryReadIds(string body, out int contactId, out int recallId)
    {
        contactId = recallId = 0;
        var parts = body.Split(new[] { ' ', ',', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && int.TryParse(parts[0], out contactId) && contactId > 0
            && int.TryParse(parts[1], out recallId) && recallId > 0;
    }

    public class SendMessageRequest { public string Message { get; set; } = ""; }

    [Authorize]
    [HttpPost("SendMessage/{contactId}/{recallId}")]
    public IActionResult SendMessage(int contactId, int recallId, [FromBody] SendMessageRequest request)
    {
        try
        {
            _messages.SendMessageByID(contactId, recallId, request.Message);
            return Ok(new { message = "Message submitted to the SMS provider." });
        }
        catch (ArgumentException error) { return BadRequest(new { message = error.Message }); }
        // Provider failures are handled by the generic exception handler, never exposed with secrets.
    }
}
