using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public interface IMessageService
{
    void SendMessageByID(int contactId, int recallId, string body);
    void SendMessage(string recipient, string body);
}

public class MessageService : IMessageService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public MessageService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public void SendMessageByID(int contactId, int recallId, string body)
    {
        var recipient = _context.RecallRecipients.AsNoTracking().Include(r => r.Recall)
            .SingleOrDefault(r => r.recallId == recallId && r.contactId == contactId);
        if (recipient == null || recipient.Recall.active != 1)
            throw new ArgumentException("The contact must belong to an active recall.");
        // Always send the saved recall message and snapshotted destination, not arbitrary client content.
        SendMessage(recipient.PhoneNumber, "A recall has been initiated.\n\nMessage: " + recipient.Recall.message
            + "\n\nReply with your contact ID followed by your recall ID to acknowledge.\nYour contact ID is: "
            + contactId + ".\nYour recall ID is: " + recallId + ".");
    }

    public void SendMessage(string recipient, string body)
    {
        var accountSid = _configuration["Twilio:AccountSid"] ?? throw new InvalidOperationException("SMS configuration is missing.");
        var authToken = _configuration["Twilio:AuthToken"] ?? throw new InvalidOperationException("SMS configuration is missing.");
        var from = _configuration["Twilio:FromNumber"] ?? throw new InvalidOperationException("SMS sender configuration is missing.");
        TwilioClient.Init(accountSid, authToken);
        MessageResource.Create(new CreateMessageOptions(new PhoneNumber(PhoneNumbers.Normalize(recipient)))
        {
            From = new PhoneNumber(from), Body = body
        });
    }
}
