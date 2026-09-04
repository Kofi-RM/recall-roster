using System.Text.Json.Serialization;

namespace recall_roster.Models;

// A recall's recipients never follow later edits to its source roster or contacts.
public class RecallRecipient
{
    public int recallId { get; set; }
    public int contactId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Rank { get; set; } = "";
    [JsonIgnore] public Recall Recall { get; set; } = null!;
}
