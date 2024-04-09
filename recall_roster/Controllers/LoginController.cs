using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Threading.Tasks;

namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
         private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, AppDbContext dbContext)
        {
            _dbContext = dbContext;
             _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                // Find the user with the provided email
                var user = await _dbContext.Logins.FirstOrDefaultAsync(u => u.Email == loginModel.Email && u.Password == loginModel.Password );

                // Check if user exists
                if (user != null)
                {
                    // User found, return success
                    return Ok("Login successful.");
                }
                else
                {
                    // User not found
                    return BadRequest("Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                 _logger.LogError(ex, "An error occurred while processing the login request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
