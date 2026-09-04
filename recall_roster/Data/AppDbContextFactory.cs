using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace recall_roster.Data;

// Migration generation must not start the web host, expose secrets, or connect to a live DB.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RecallRosterDesignOnly;Trusted_Connection=True;").Options;
        return new AppDbContext(options, new ConfigurationBuilder().Build());
    }
}
