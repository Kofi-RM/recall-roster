using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Services;

public class MessageService : IMessageService
{
    private readonly AppDbContext _context;
    private readonly ISmsSender _sender;
    public MessageService(AppDbContext context, ISmsSender sender)
    {
        _context = context;
        _sender = sender;
    }

    public void SendMessageByID(int contactId, int recallId, string body)
    {
        var recipient = _context.RecallRecipients.AsNoTracking().Include(r => r.Recall)
            .SingleOrDefault(r => r.recallId == recallId && r.contactId == contactId);
        if (recipient == null || recipient.Recall.active != 1)
            throw new ArgumentException("The contact must belong to an active recall.");
        if (!_context.Access.CanInitiate(recipient.Recall.UnitId)) throw new UnauthorizedAccessException("Recall initiation permission is required.");
        // Always send the saved recall message and snapshotted destination, not arbitrary client content.
        _sender.Send(recipient.PhoneNumber, "A recall has been initiated.\n\nMessage: " + recipient.Recall.message
            + "\n\nReply with your contact ID followed by your recall ID to acknowledge.\nYour contact ID is: "
            + contactId + ".\nYour recall ID is: " + recallId + ".");
    }

}
