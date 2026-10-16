using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
namespace recall_roster.Services;

public sealed class UnitAccessService(AppDbContext db, UnitAccessScope scope)
{
    public async Task<bool> Load(ClaimsPrincipal principal)
    {
        scope.IsSystem = false; scope.IsAdministrator = false; scope.UserId = null;
        scope.VisibleUnitIds = scope.ManagedUnitIds = scope.RecallUnitIds = Array.Empty<int>();
        if (!int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub"), out var id)) return false;
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id && u.IsEnabled);
        if (user == null) return false;
        var versionClaim = principal.FindFirstValue("token_version") ?? "0";
        if (!int.TryParse(versionClaim, out var version) || version != user.TokenVersion) return false;
        scope.UserId = id;
        scope.IsAdministrator = user.IsSystemAdministrator;
        var memberships = await db.UnitMemberships.AsNoTracking().Where(m => m.UserId == id).ToListAsync();
        var units = await db.Units.AsNoTracking().ToListAsync();
        scope.VisibleUnitIds = Expand(units, memberships).ToArray();
        scope.ManagedUnitIds = Expand(units, memberships.Where(m => m.CanManageRosters)).ToArray();
        scope.RecallUnitIds = Expand(units, memberships.Where(m => m.CanInitiateRecalls)).ToArray();
        return true;
    }

    private static HashSet<int> Expand(List<Unit> units, IEnumerable<UnitMembership> grants)
    {
        var result = new HashSet<int>();
        foreach (var grant in grants)
        {
            if (!units.Any(u => u.Id == grant.UnitId)) continue;
            result.Add(grant.UnitId);
            if (!grant.IncludeDescendants) continue;
            var descendants = new HashSet<int> { grant.UnitId };
            bool changed;
            do
            {
                changed = false;
                foreach (var unit in units)
                    if (unit.ParentUnitId.HasValue && descendants.Contains(unit.ParentUnitId.Value)) changed |= descendants.Add(unit.Id);
            } while (changed);
            result.UnionWith(descendants);
        }
        return result;
    }
}
