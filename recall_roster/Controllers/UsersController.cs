using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;

namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
         private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, AppDbContext dbContext)
        {
            _dbContext = dbContext;
             _logger = logger;
        }

// Get one user
[Authorize]
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id)
{
    try
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}
// get all users
[Authorize]
   [HttpGet]
public async Task<IActionResult> GetAllUsers()
{
    try
    {
        var users = await _dbContext.Users
            .Select(u => new
            {
                u.Id,
                u.Email
            })
            .ToListAsync();

        return Ok(users);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while retrieving users.");
        return StatusCode(500, "An error occurred while processing your request.");
    }
}
[Authorize]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null) return NotFound();

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return StatusCode(0);
                
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


//
    
}


    }

