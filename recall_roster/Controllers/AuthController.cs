using System.Drawing.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
using recall_roster.Services;



[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthController> _logger;
    private readonly JwtService _jwtService;
    public AuthController(AppDbContext dbContext, ILogger<AuthController> logger, JwtService jwtService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
                return BadRequest("User already exists.");

            var user = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, "Server error");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isValid)
                return Unauthorized("Invalid email or password.");

          var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            message = "Login successful",
            token,
            user = new
            {
                user.Id,
                user.Email
            }
        });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "Server error");
        }
    }
}

  