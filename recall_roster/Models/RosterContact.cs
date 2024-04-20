namespace recall_roster.Models
{
    public class RosterContact
    {
       public int rosterId { get; set; }
    public Roster? Roster { get; set; } // Add nullable reference type
    public int contactId { get; set; }
    public Contact? Contact { get; set; } // Add nullable reference type
      
    }
}
