using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

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

// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using recall_roster.Data;
// using recall_roster.Models;
// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;

// namespace recall_roster.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class LoginController : ControllerBase
//     {
//         private readonly AppDbContext _dbContext;
//         private readonly IConfiguration _configuration;
// private readonly string _secretKey;
//         public LoginController(AppDbContext dbContext, IConfiguration configuration)
//         {
//             _dbContext = dbContext;
//             _configuration = configuration;
//             _secretKey = configuration["Jwt:SecretKey"];
//         }

//         [HttpPost]
//         public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
//         {
//             try
//             {
//                 // Find the user with the provided email
//                 var user = await _dbContext.Logins.FirstOrDefaultAsync(u => u.Email == loginModel.Email && u.Password == loginModel.Password);

//                 // Check if user exists
//                 if (user != null)
//                 {
//                     // User found, generate JWT token
//                     var token = GenerateJwtToken(user);

//                     // Return success with JWT token
//                     return Ok(new { Token = token });
//                 }
//                 else
//                 {
//                     // User not found
//                     return BadRequest("Invalid email or password.");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // Log the exception
//                 // Handle exceptions appropriately based on your application's logging strategy
//                 return StatusCode(500, "An error occurred while processing your request.");
//             }
//         }

//         private string GenerateJwtToken(LoginModel user)
//         {
//             var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
//             var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//             var claims = new[]
//             {
//                 new Claim(JwtRegisteredClaimNames.Sub, user.Email),
//                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//             };

//             var token = new JwtSecurityToken(
//                 issuer: _configuration["Jwt:Issuer"],
//                 audience: _configuration["Jwt:Audience"],
//                 claims: claims,
//                 expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpirationInMinutes"])),
//                 signingCredentials: credentials);

//             return new JwtSecurityTokenHandler().WriteToken(token);
//         }
//     }
// }
