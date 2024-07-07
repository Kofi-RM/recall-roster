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

    public List<Contact> GetAllContacts()
    {
       return _context.Contacts.Where(c => c.Active == 1).ToList();
    }
    
    public void RemoveContact(Contact contact)
    {
        contact.Active = 0;
        _context.SaveChanges();
    }

    public void AddContact(Contact contact)
{
    _context.Contacts.Add(contact);
    _context.SaveChanges();
}

    public void UpdateContact(Contact updatedContact)
        {
            var existingContact = _context.Contacts.FirstOrDefault(c => c.contactID == updatedContact.contactID);
            if (existingContact != null)
            {
                existingContact.FirstName = updatedContact.FirstName;
                existingContact.LastName = updatedContact.LastName;
                
                existingContact.PhoneNumber = updatedContact.PhoneNumber;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Contact not found");
            }
        }

}

public interface IContactService
{
    Contact? GetContactById(int contactId);
    List<Contact>? GetAllContacts();
    void RemoveContact(Contact contact);
    void AddContact(Contact contact);
    void UpdateContact(Contact updatedContact);
}
