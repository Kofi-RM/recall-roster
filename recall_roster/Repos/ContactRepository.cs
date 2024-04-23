using recall_roster.Data;
using recall_roster.Models;

namespace recall_roster.Repos;
public class ContactRepository
{
    private readonly AppDbContext _dbContext;

    public ContactRepository(AppDbContext dbContext)
    {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    }

    public List<Contact> GetAllContacts()
    {
       return _dbContext.Contacts.Where(c => c.Active == 1).ToList();
    }

     public Contact GetContact(int id)
    {
        var output = this._dbContext.Contacts.FirstOrDefault(o => o.contactID == id);
        return output;
    }

       public void RemoveContact(Contact contact)
    {
     

    // If the contact exists in the database
  
        // Set the active property to 0
        contact.Active = 0;

        // Update the contact entity in the database
        _dbContext.SaveChanges();
    }

  public void AddContact(Contact contact)
{
    _dbContext.Contacts.Add(contact);
    _dbContext.SaveChanges();
}

 public void UpdateContact(Contact updatedContact)
        {
            var existingContact = _dbContext.Contacts.FirstOrDefault(c => c.contactID == updatedContact.contactID);
            if (existingContact != null)
            {
                existingContact.FirstName = updatedContact.FirstName;
                existingContact.LastName = updatedContact.LastName;
                
                existingContact.PhoneNumber = updatedContact.PhoneNumber;

                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Contact not found");
            }

    // Add other methods for CRUD operations as needed...
        }
}