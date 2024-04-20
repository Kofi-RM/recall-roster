using recall_roster.Models;

public class UpdateRosterContacts
{
    public int RosterId { get; set; }
    public List<Contact> ContactsToAdd { get; set; }
    public List<Contact> ContactsToRemove { get; set; }
}