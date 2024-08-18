using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using Twilio.AspNet.Core;
using System;
using Microsoft.EntityFrameworkCore.Internal;

namespace recall_roster.Controllers{
[ApiController]
[Route("api/[controller]")]
public class MessageController : TwilioController
{
    private readonly IContactService _contactService; 
    private readonly IMessageService _messageService;

    private readonly IResponseService _responseService;

    public MessageController(IContactService contactService, IMessageService messageService, IResponseService responseService) {
        _contactService = contactService;
        _messageService = messageService;
        _responseService = responseService;
    }
    private int[] ExtractIds(string message) {
    // Split the message by space, comma, or any other separator
    try
    {
        // Split the message by space, comma, or any other separator
        var parts = message.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

        Console.WriteLine(string.Join(", ", parts));  // Print parts array to ensure message is split correctly

        if (parts.Length < 2)
        {
            Console.WriteLine("Message does not contain enough numbers.");
            return null; // Early return or handle invalid input
        }

        // Try to parse two numbers from the message
        int.TryParse(parts[0], out int firstNumber);
        int.TryParse(parts[1], out int secondNumber);



        return new[] { firstNumber, secondNumber };
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception occurred: " + ex.Message);
        return null; // Handle exceptions gracefully
    }
    }

  
    

    [HttpPost("ReceiveMessage")]
    public TwiMLResult ReceiveSms([FromForm] string From, [FromForm] string Body)
    {

    
        Console.WriteLine($"Received message from {From}: {Body}");
        
        if (string.IsNullOrWhiteSpace(Body))
        {
            Console.WriteLine("Received empty or null message body.");
            return new TwiMLResult(new MessagingResponse());
        }

        var num = ExtractIds(Body);

        if (num == null || num.Length < 2)
        {
            Console.WriteLine("Failed to parse valid numbers from the message.");
            return new TwiMLResult(new MessagingResponse());
        }

      

        var contact = _contactService.GetContactByNumber(From);
       
        if (contact.contactID == num[0])
        {
            Console.WriteLine("message Controller");
            
            _responseService.AddResponse(From, Body, num[1]);
        }
    
    
    

        // Respond back to Twilio. In this case, no message is sent back to the user.
        var response = new MessagingResponse();
        return TwiML(response);
    }

    [HttpPost("SendMessage/{contactId}/{recallId}")]
    public ActionResult SendMessage(int contactId, int recallId)
    {
        try
        {
            _messageService.SendMessageByID(contactId, recallId);
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
