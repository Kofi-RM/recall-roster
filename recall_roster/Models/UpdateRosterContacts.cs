using recall_roster.Models;

public class UpdateRosterContacts
{
    public int RosterId { get; set; }
    public int[] ContactsToAdd { get; set; } = Array.Empty<int>();
    public int[] ContactsToRemove { get; set; } = Array.Empty<int>();
}
