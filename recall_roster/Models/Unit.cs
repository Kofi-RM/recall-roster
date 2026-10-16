using System.ComponentModel.DataAnnotations;
namespace recall_roster.Models;

public sealed class Unit
{
    public int Id { get; set; }
    [Required, MaxLength(160)] public string Name { get; set; } = "";
    public int? ParentUnitId { get; set; }
}

public sealed class UnitMembership
{
    public int UserId { get; set; }
    public int UnitId { get; set; }
    // Membership itself grants read access. Operational privileges are explicit.
    public bool CanManageRosters { get; set; }
    public bool CanInitiateRecalls { get; set; }
    public bool IncludeDescendants { get; set; }
}

public sealed class AccessAudit
{
    public int Id { get; set; }
    public int? ActorUserId { get; set; }
    [MaxLength(100)] public string Action { get; set; } = "";
    [MaxLength(2000)] public string Details { get; set; } = "";
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
