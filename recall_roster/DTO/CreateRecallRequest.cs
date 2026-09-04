using System.ComponentModel.DataAnnotations;

namespace recall_roster.DTOs;

public class CreateRecallRequest
{
    [Range(1, int.MaxValue)] public int rosterId { get; set; }
    [Required, StringLength(1600, MinimumLength = 1)] public string message { get; set; } = "";
    public DateTimeOffset timeEnded { get; set; }
}
