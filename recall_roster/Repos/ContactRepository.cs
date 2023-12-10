using recall_roster.Data;
using recall_roster.Models;
using System.Collections.Generic;
using System.Linq;

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
        return _dbContext.Contacts.ToList();
    }

    // Add other methods for CRUD operations as needed...
}
