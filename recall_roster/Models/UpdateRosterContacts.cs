using recall_roster.Models;

public class UpdateRosterContacts
{
    public int RosterId { get; set; }
    public int[] ContactsToAdd { get; set; }
    public int[] ContactsToRemove { get; set; }
}