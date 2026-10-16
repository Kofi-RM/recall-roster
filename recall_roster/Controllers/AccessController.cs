using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
using recall_roster.Services;
namespace recall_roster.Controllers;

[Authorize, ApiController, Route("api/access")]
public sealed class AccessController(AppDbContext db, UnitAccessScope scope) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me() => Ok(new
    {
        userId = scope.UserId, isAdministrator = scope.IsAdministrator,
        units = (await db.Units.AsNoTracking().Where(u => scope.VisibleUnitIds.Contains(u.Id)).OrderBy(u => u.Name).ToListAsync())
            .Select(u => new { u.Id, u.Name, u.ParentUnitId, canManageRosters = scope.CanManage(u.Id), canInitiateRecalls = scope.CanInitiate(u.Id) })
    });

    [HttpGet("units")]
    public async Task<IActionResult> Units() => Ok(await db.Units.AsNoTracking().OrderBy(u => u.Name).ToListAsync());

    public record CreateUnit(string Name, int? ParentUnitId);
    [HttpPost("units")]
    public async Task<IActionResult> AddUnit(CreateUnit input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 160) return BadRequest(new { message = "A unit name of at most 160 characters is required." });
        if (input.ParentUnitId.HasValue && !await db.Units.AnyAsync(u => u.Id == input.ParentUnitId)) return BadRequest(new { message = "Parent unit not found." });
        var unit = new Unit { Name = input.Name.Trim(), ParentUnitId = input.ParentUnitId };
        db.Units.Add(unit);
        Audit("Create unit", $"Name={unit.Name}; Parent={unit.ParentUnitId}");
        await db.SaveChangesAsync();
        return Ok(unit);
    }

    public record RenameUnit(string Name);
    [HttpPut("units/{id:int}")]
    public async Task<IActionResult> UpdateUnit(int id, RenameUnit input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 160) return BadRequest(new { message = "A unit name of at most 160 characters is required." });
        var unit = await db.Units.FindAsync(id);
        if (unit == null) return NotFound();
        unit.Name = input.Name.Trim();
        Audit("Rename unit", $"Unit={id}; Name={unit.Name}");
        await db.SaveChangesAsync();
        return Ok(unit);
    }

    [HttpGet("memberships")]
    public async Task<IActionResult> Memberships() => Ok(await db.UnitMemberships.AsNoTracking().ToListAsync());
    public record MembershipRequest(bool CanManageRosters, bool CanInitiateRecalls, bool IncludeDescendants);
    [HttpPut("memberships/{userId:int}/{unitId:int}")]
    public async Task<IActionResult> Grant(int userId, int unitId, MembershipRequest input)
    {
        if (!await db.Users.AnyAsync(u => u.Id == userId && u.IsEnabled) || !await db.Units.AnyAsync(u => u.Id == unitId)) return NotFound();
        var membership = await db.UnitMemberships.FindAsync(userId, unitId);
        if (membership == null) { membership = new UnitMembership { UserId = userId, UnitId = unitId }; db.UnitMemberships.Add(membership); }
        membership.CanManageRosters = input.CanManageRosters;
        membership.CanInitiateRecalls = input.CanInitiateRecalls;
        membership.IncludeDescendants = input.IncludeDescendants;
        Audit("Grant membership", $"User={userId}; Unit={unitId}; Manage={input.CanManageRosters}; Initiate={input.CanInitiateRecalls}; Descendants={input.IncludeDescendants}");
        await db.SaveChangesAsync();
        return Ok(membership);
    }

    [HttpDelete("memberships/{userId:int}/{unitId:int}")]
    public async Task<IActionResult> Revoke(int userId, int unitId)
    {
        var membership = await db.UnitMemberships.FindAsync(userId, unitId);
        if (membership == null) return NotFound();
        db.UnitMemberships.Remove(membership);
        Audit("Revoke membership", $"User={userId}; Unit={unitId}");
        await db.SaveChangesAsync();
        return NoContent();
    }

    public record UserPermissions(bool IsSystemAdministrator, bool IsEnabled);
    [HttpPut("users/{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UserPermissions input)
    {
        // Serialize privilege changes so concurrent demotions cannot remove the last administrator.
        using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();
        if (user.IsSystemAdministrator && user.IsEnabled && (!input.IsEnabled || !input.IsSystemAdministrator)
            && await db.Users.CountAsync(u => u.IsSystemAdministrator && u.IsEnabled) <= 1)
            return BadRequest(new { message = "Keep at least one enabled administrator." });
        if (input.IsEnabled && !System.Text.RegularExpressions.Regex.IsMatch(user.Password ?? "", @"^\$2[aby]\$[0-9]{2}\$[./A-Za-z0-9]{53}$"))
            return BadRequest(new { message = "Reset this account’s unsupported legacy password before enabling it." });
        user.IsSystemAdministrator = input.IsSystemAdministrator;
        if (user.IsEnabled != input.IsEnabled) user.TokenVersion++;
        user.IsEnabled = input.IsEnabled;
        Audit("Update user permissions", $"User={id}; Administrator={input.IsSystemAdministrator}; Enabled={input.IsEnabled}");
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return Ok(new { user.Id, user.Email, user.IsEnabled, user.IsSystemAdministrator });
    }

    public record PasswordReset(string Password);
    [HttpPut("users/{id:int}/password")]
    public async Task<IActionResult> ResetPassword(int id, PasswordReset input)
    {
        if (string.IsNullOrEmpty(input.Password) || input.Password.Length < 12 || input.Password.Length > 128)
            return BadRequest(new { message = "Use a password between 12 and 128 characters." });
        using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
        user.TokenVersion++;
        Audit("Reset account password", $"User={id}; enabled status unchanged");
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return NoContent();
    }

    [HttpGet("audit")]
    public async Task<IActionResult> History() => Ok(await db.AccessAudits.AsNoTracking().OrderByDescending(a => a.Id).Take(200).ToListAsync());
    private void Audit(string action, string details) => db.AccessAudits.Add(new AccessAudit { ActorUserId = scope.UserId, Action = action, Details = details });
}
