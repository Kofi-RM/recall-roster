using recall_roster.Data;
using recall_roster.Models;

public class ContactService : IContactService
{
     private readonly AppDbContext _context;

    public ContactService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public Contact? GetContactById(int contactId)
    {
        var contact = _context.Contacts.FirstOrDefault(c => c.contactID == contactId);
        return contact;
    }
}

public interface IContactService
{
    Contact? GetContactById(int contactId);
}
