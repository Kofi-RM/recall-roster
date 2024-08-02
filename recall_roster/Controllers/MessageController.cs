using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using Twilio.AspNet.Core;

namespace recall_roster.Controllers{
[ApiController]
[Route("api/[controller]")]
public class MessageController : TwilioController
{
    private readonly IContactService _contactService; 
    private readonly IMessageService _messageService;

    public MessageController(IContactService contactService, IMessageService messageService) {
        _contactService = contactService;
        _messageService = messageService;
    }

    [HttpPost("ReceiveMessage")]
    public TwiMLResult ReceiveSms([FromForm] string From, [FromForm] string Body)
    {
        Console.WriteLine($"Received message from {From}: {Body}");

        // Try to parse the body as a contact ID
        if (int.TryParse(Body, out int contactId))
        {
            var contact = _contactService.GetContactById(contactId);
            if (contact != null)
            {
                ResponseCollector.AddResponse(contact.Rank);
            }
        }

        // Respond back to Twilio. In this case, no message is sent back to the user.
        var response = new MessagingResponse();
        return TwiML(response);
    }

    [HttpPost("SendMessage/{contactId}")]
    public ActionResult SendMessage(int contactId)
    {
        try
        {
            _messageService.SendMessageByID(contactId);
            return Ok("Message sent successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to send message: {ex.Message}");
        }
    }

    [HttpPost("SendMessage")]
    public ActionResult Send() 
        {
    
        try 
        {
         _messageService.SendMessage("4707862142", "We go");
        return Ok("Message sent successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to send message: {ex.Message}");
        }
}
}
}
