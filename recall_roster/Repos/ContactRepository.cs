using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

     public Contact GetContact(int id)
    {
        var output = this._dbContext.Contacts.FirstOrDefault(o => o.contactID == id);
        return output;
    }

       public void RemoveContact(Contact name)
    {
        this._dbContext.Contacts.Remove(name);
       this._dbContext.SaveChanges();
    }

  public void AddContact(Contact contact)
{
    _dbContext.Contacts.Add(contact);
    _dbContext.SaveChanges();
}

    // Add other methods for CRUD operations as needed...
}
