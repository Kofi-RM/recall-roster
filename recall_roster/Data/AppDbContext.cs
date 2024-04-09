// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using recall_roster.Models;

namespace recall_roster.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Contact> Contacts { get; set; }
          public DbSet<LoginModel> Logins { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Check if the configuration is already set (it will be in production when using dependency injection)
        if (!optionsBuilder.IsConfigured)
        {
            // Use the configuration to get the connection string from appsettings.json
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Configure the database connection
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}