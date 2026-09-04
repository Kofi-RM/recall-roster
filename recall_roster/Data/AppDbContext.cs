using Microsoft.EntityFrameworkCore;
using recall_roster.Models;

namespace recall_roster.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options) { }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Roster> Rosters => Set<Roster>();
    public DbSet<RosterContact> RosterContacts => Set<RosterContact>();
    public DbSet<Recall> Recalls => Set<Recall>();
    public DbSet<Response> Responses => Set<Response>();
    public DbSet<RecallRecipient> RecallRecipients => Set<RecallRecipient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Contact>().Property(c => c.contactId).HasColumnName("contactID");
        modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(320);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<RosterContact>().ToTable("RosterContact");
        modelBuilder.Entity<RosterContact>().HasKey(rc => new { rc.rosterId, rc.contactId });
        modelBuilder.Entity<RosterContact>().HasOne(rc => rc.Roster).WithMany(r => r.RosterContacts)
            .HasForeignKey(rc => rc.rosterId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RosterContact>().HasOne(rc => rc.Contact).WithMany(c => c.RosterContacts)
            .HasForeignKey(rc => rc.contactId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recall>().Property(r => r.Total)
            .HasComputedColumnSql("[Employees] + [ElementChief] + [FlightChief] + [SquadronDirector]");
        modelBuilder.Entity<RecallRecipient>().HasKey(r => new { r.recallId, r.contactId });
        modelBuilder.Entity<RecallRecipient>().HasOne(r => r.Recall).WithMany(r => r.Recipients)
            .HasForeignKey(r => r.recallId).OnDelete(DeleteBehavior.Restrict);
        // Deliberately no link back to live Contacts/Rosters: snapshots survive their removal.
        modelBuilder.Entity<Response>().HasOne(r => r.Contact).WithMany()
            .HasForeignKey(r => r.contactId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Response>().HasOne(r => r.Recall).WithMany()
            .HasForeignKey(r => r.recallId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Response>().HasIndex(r => new { r.recallId, r.contactId })
            .IsUnique().HasFilter("[IsDuplicate] = 0");
    }
}
