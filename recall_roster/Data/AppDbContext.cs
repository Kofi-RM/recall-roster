using Microsoft.EntityFrameworkCore;
using recall_roster.Models;

namespace recall_roster.Data
{
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
        public DbSet<Roster> Rosters { get; set; }
        public DbSet<RosterContact> RosterContacts { get; set; }

        public DbSet<Recall> Recalls {get; set;}
        
        public DbSet<Response> Responses {get; set;}

     protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Define Roster_Contact entity
    modelBuilder.Entity<RosterContact>()
        .HasKey(rc => new { rc.rosterId, rc.contactId }); // Define composite primary key

    // Define foreign key constraints and navigation properties
    modelBuilder.Entity<RosterContact>()
        .HasOne(rc => rc.Roster)                 // Roster_Contact has one Roster
        .WithMany(r => r.RosterContacts)         // Roster has many Roster_Contacts
        .HasForeignKey(rc => rc.rosterId);      // Foreign key

    modelBuilder.Entity<RosterContact>()
        .HasOne(rc => rc.Contact)                // Roster_Contact has one Contact
        .WithMany(c => c.RosterContacts)         // Contact has many Roster_Contacts
        .HasForeignKey(rc => rc.contactId);     // Foreign key

    // You might also need to specify the table name if it's different from the convention
    modelBuilder.Entity<RosterContact>().ToTable("RosterContact");
}
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
}
