namespace recall_roster.Services;

// Scoped to one request. Default deny; only the verified webhook/bootstrap and tests use system scope.
public sealed class UnitAccessScope
{
    public bool IsSystem { get; set; }
    public bool IsAdministrator { get; set; }
    public int? UserId { get; set; }
    public int[] VisibleUnitIds { get; set; } = Array.Empty<int>();
    public int[] ManagedUnitIds { get; set; } = Array.Empty<int>();
    public int[] RecallUnitIds { get; set; } = Array.Empty<int>();
    public bool CanManage(int unitId) => IsSystem || ManagedUnitIds.Contains(unitId);
    public bool CanInitiate(int unitId) => IsSystem || RecallUnitIds.Contains(unitId);
}
