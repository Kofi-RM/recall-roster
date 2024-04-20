using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace recall_roster.Repos
{
    public class RosterContactRepository
    {
        private readonly AppDbContext _dbContext;

        public RosterContactRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Method to retrieve all roster contacts
        public List<RosterContact> GetAllRosterContacts()
        {
            return _dbContext.RosterContacts.ToList();
        }

        // Method to retrieve roster contacts by roster ID
        public RosterContact GetRosterContact(int rosterId)
        {
            return _dbContext.RosterContacts.FirstOrDefault(rc => rc.rosterId == rosterId);
        }

        // Method to add roster contact
        public void AddRosterContact(RosterContact rosterContact)
        {
            _dbContext.RosterContacts.Add(rosterContact);
            _dbContext.SaveChanges();
        }

        // Method to remove roster contact
        public void RemoveRosterContact(RosterContact rosterContact)
        {
            _dbContext.RosterContacts.Remove(rosterContact);
            _dbContext.SaveChanges();
        }

        public void UpdateRosterContacts(int rosterId, List<Contact> contactsToAdd, List<Contact> contactsToRemove)
{
    // Add new roster contacts
    foreach (var contact in contactsToAdd)
    {
        var rosterContactToAdd = new RosterContact
        {
            rosterId = rosterId,
            contactId = contact.contactID // Assuming Contact class has an Id property
        };
        _dbContext.RosterContacts.Add(rosterContactToAdd);
    }

    // Remove existing roster contacts
    foreach (var contact in contactsToRemove)
    {
        var rosterContactToRemove = _dbContext.RosterContacts.FirstOrDefault(rc => rc.rosterId == rosterId && rc.contactId == contact.contactID);
        if (rosterContactToRemove != null)
        {
            _dbContext.RosterContacts.Remove(rosterContactToRemove);
        }
    }

    // Save changes to the database
    _dbContext.SaveChanges();
}
        // Add other methods for CRUD operations as needed...
    }
}
