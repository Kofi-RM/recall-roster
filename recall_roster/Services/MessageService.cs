using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public interface IMessageService
{
    void SendMessageByID(int contactId);
    void SendMessage(string name, string content);
}

public class MessageService : IMessageService
{
    private readonly IContactService _contactService;
    private readonly IConfiguration _configuration;

    public MessageService(IContactService contactService, IConfiguration configuration)
    {
        _contactService = contactService;
        _configuration = configuration;
    }

    public void SendMessageByID(int contactId)
    {
        var contact = _contactService.GetContactById(contactId);
        if (contact != null)
        {
            Console.WriteLine($"Sending message to {contact.FirstName} {contact.LastName}");
            SendMessage(contact.PhoneNumber, "A recall has been initiated. Please respond with your contact ID to confirm your presence.");
        }
        else
        {
            Console.WriteLine("Contact not found.");
        }
    }

    public void SendMessage(string recipient, string body)
    {
        var accountSid = _configuration["Twilio:AccountSid"];
        var authToken = _configuration["Twilio:AuthToken"];
        TwilioClient.Init(accountSid, authToken);

        var messageOptions = new CreateMessageOptions(new PhoneNumber($"+1{recipient}"))
        {
            From = new PhoneNumber("+18336223946"),
            Body = body
        };

        var message = MessageResource.Create(messageOptions);
        Console.WriteLine($"Message SID: {message.Sid}");
    }
}
