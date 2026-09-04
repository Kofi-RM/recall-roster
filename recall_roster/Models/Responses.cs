using System.Text.Json.Serialization;

namespace recall_roster.Models;

public class Response
{
    public int responseId { get; set; }
    public int contactId { get; set; }
    public int recallId { get; set; }
    public DateTime responseTime { get; set; }
    public string response { get; set; } = "";
    // Migration preserves duplicate historical rows, excluding them from acknowledgments.
    public bool IsDuplicate { get; set; }
    [JsonIgnore] public Contact Contact { get; set; } = null!;
    [JsonIgnore] public Recall Recall { get; set; } = null!;
}
