using System.Data;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;

public class RosterContactService : IRosterContactService
{
    private readonly AppDbContext _context;
    public RosterContactService(AppDbContext context) => _context = context;
    public List<RosterContact> GetAllRosterContacts(int rosterId) =>
        _context.RosterContacts.AsNoTracking().Where(rc => rc.rosterId == rosterId).ToList();
    public RosterContact? GetRosterContact(int rosterId, int contactId) =>
        _context.RosterContacts.FirstOrDefault(rc => rc.rosterId == rosterId && rc.contactId == contactId);
    public void AddRosterContact(RosterContact contact) =>
        UpdateRosterContacts(contact.rosterId, new[] { contact.contactId }, Array.Empty<int>());
    public void RemoveRosterContact(RosterContact contact) =>
        UpdateRosterContacts(contact.rosterId, Array.Empty<int>(), new[] { contact.contactId });

    public void UpdateRosterContacts(int rosterId, int[] contactsToAdd, int[] contactsToRemove)
    {
        var added = (contactsToAdd ?? Array.Empty<int>()).Distinct().ToArray();
        var removed = (contactsToRemove ?? Array.Empty<int>()).Distinct().ToArray();
        if (added.Intersect(removed).Any() || added.Concat(removed).Any(id => id <= 0))
            throw new ArgumentException("Contact IDs must be positive and cannot be both added and removed.");
        using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
        if (!_context.Rosters.Any(r => r.rosterId == rosterId))
            throw new ArgumentException("Roster not found.");
        if (_context.Contacts.Count(c => added.Contains(c.contactId) && c.Active == 1) != added.Length)
            throw new ArgumentException("Only existing active contacts can be added.");
        var existing = _context.RosterContacts.Where(rc => rc.rosterId == rosterId).ToList();
        foreach (var id in added.Except(existing.Select(rc => rc.contactId)))
            _context.RosterContacts.Add(new RosterContact { rosterId = rosterId, contactId = id });
        _context.RosterContacts.RemoveRange(existing.Where(rc => removed.Contains(rc.contactId)));
        _context.SaveChanges();
        transaction.Commit();
    }
}

public interface IRosterContactService
{
    List<RosterContact> GetAllRosterContacts(int rosterId);
    RosterContact? GetRosterContact(int rosterId, int contactId);
    void AddRosterContact(RosterContact rosterContact);
    void RemoveRosterContact(RosterContact rosterContact);
    void UpdateRosterContacts(int rosterId, int[] contactsToAdd, int[] contactsToRemove);
}
