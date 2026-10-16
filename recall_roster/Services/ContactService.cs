using recall_roster.Data;
using recall_roster.Models;
using recall_roster.DTOs;

public sealed class ContactService : IContactService
{
    private readonly AppDbContext _context;
    public ContactService(AppDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));
    public Contact? GetContactById(int contactId) => _context.Contacts.FirstOrDefault(c => c.contactId == contactId);
    public Contact? GetContactByNumber(string number) => _context.Contacts.FirstOrDefault(c => c.PhoneNumber == number);
    public List<Contact> GetAllContacts() => _context.Contacts.Where(c => c.Active == 1).ToList();

    public void RemoveContact(Contact contact)
    {
        var existing = GetContactById(contact.contactId) ?? throw new KeyNotFoundException("Contact not found.");
        existing.Deactivate();
        _context.SaveChanges();
    }

    public Contact AddContact(ContactCreateDto input)
    {
        var contact = new Contact { FirstName = "", LastName = "", PhoneNumber = "", Rank = "", Active = 1, UnitId = input.UnitId };
        contact.UpdateDetails(input.FirstName, input.LastName, input.PhoneNumber, input.Rank);
        _context.Contacts.Add(contact);
        _context.SaveChanges();
        return contact;
    }

    public Contact UpdateContact(Contact input)
    {
        var existing = GetContactById(input.contactId) ?? throw new KeyNotFoundException("Contact not found.");
        existing.UpdateDetails(input.FirstName, input.LastName, input.PhoneNumber, input.Rank);
        _context.SaveChanges();
        return existing;
    }
}
