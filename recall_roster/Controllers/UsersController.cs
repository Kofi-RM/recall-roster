using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
namespace recall_roster.Controllers;

[Authorize, ApiController, Route("api/User")]
public sealed class UserController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllUsers() => Ok(await db.Users.AsNoTracking()
        .Select(u => new { u.Id, u.Email, u.IsSystemAdministrator, u.IsEnabled }).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await db.Users.AsNoTracking().Where(u => u.Id == id)
            .Select(u => new { u.Id, u.Email, u.IsSystemAdministrator, u.IsEnabled }).SingleOrDefaultAsync();
        return user == null ? NotFound() : Ok(user);
    }
    // Disable accounts through /api/access/users/{id}; deleting an account would erase attribution.
}
