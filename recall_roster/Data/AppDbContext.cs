using Microsoft.EntityFrameworkCore;
using recall_roster.Models;
using recall_roster.Services;

namespace recall_roster.Data;

public class AppDbContext : DbContext
{
    public UnitAccessScope Access { get; }
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration, UnitAccessScope? access = null) : base(options)
        => Access = access ?? new UnitAccessScope();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitMembership> UnitMemberships => Set<UnitMembership>();
    public DbSet<AccessAudit> AccessAudits => Set<AccessAudit>();

    // Values are read from the current context, never captured from the first cached EF model.
    private bool SystemAccess => Access.IsSystem;
    private int[] VisibleUnits => Access.VisibleUnitIds;

    private void EnforceWrites()
    {
        if (Access.IsSystem) return;
        ChangeTracker.DetectChanges();
        foreach (var entry in ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            if (entry.Entity is Contact contact)
            {
                if (!Access.CanManage(contact.UnitId) || (entry.State == EntityState.Modified && entry.Property(nameof(Contact.UnitId)).IsModified))
                    throw new UnauthorizedAccessException("Staff management permission is required; unit ownership cannot be changed by editing.");
            }
            if (entry.Entity is Roster roster)
            {
                if (!Access.CanManage(roster.UnitId) || (entry.State == EntityState.Modified && entry.Property(nameof(Roster.UnitId)).IsModified))
                    throw new UnauthorizedAccessException("Roster management permission is required; unit ownership cannot be changed by editing.");
            }
            if (entry.Entity is Recall recall && (!Access.CanInitiate(recall.UnitId) || entry.State != EntityState.Added))
                throw new UnauthorizedAccessException("Recall initiation permission is required.");
            if (entry.Entity is RosterContact link)
            {
                var rosterUnit = Rosters.Where(r => r.rosterId == link.rosterId).Select(r => (int?)r.UnitId).SingleOrDefault();
                var contactUnit = Contacts.Where(c => c.contactId == link.contactId).Select(c => (int?)c.UnitId).SingleOrDefault();
                if (!rosterUnit.HasValue || !Access.CanManage(rosterUnit.Value) || (entry.State != EntityState.Deleted && rosterUnit != contactUnit))
                    throw new UnauthorizedAccessException("Roster members must belong to the same unit and you must manage that unit.");
            }
            if (entry.Entity is Response) throw new UnauthorizedAccessException("Acknowledgments require a verified SMS webhook.");
            if (entry.Entity is User or Unit or UnitMembership or AccessAudit)
                if (!Access.IsAdministrator) throw new UnauthorizedAccessException("Administrator permission is required.");
        }
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess) { EnforceWrites(); return base.SaveChanges(acceptAllChangesOnSuccess); }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    { EnforceWrites(); return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken); }


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
        modelBuilder.Entity<Unit>().HasOne<Unit>().WithMany().HasForeignKey(u => u.ParentUnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UnitMembership>().HasKey(m => new { m.UserId, m.UnitId });
        modelBuilder.Entity<UnitMembership>().HasOne<User>().WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UnitMembership>().HasOne<Unit>().WithMany().HasForeignKey(m => m.UnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Contact>().HasOne<Unit>().WithMany().HasForeignKey(c => c.UnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Roster>().HasOne<Unit>().WithMany().HasForeignKey(r => r.UnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Recall>().HasOne<Unit>().WithMany().HasForeignKey(r => r.UnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Recall>().HasOne<User>().WithMany().HasForeignKey(r => r.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Contact>().HasQueryFilter(c => SystemAccess || VisibleUnits.Contains(c.UnitId));
        modelBuilder.Entity<Roster>().HasQueryFilter(r => SystemAccess || VisibleUnits.Contains(r.UnitId));
        modelBuilder.Entity<Recall>().HasQueryFilter(r => SystemAccess || VisibleUnits.Contains(r.UnitId));
        modelBuilder.Entity<RecallRecipient>().HasQueryFilter(r => SystemAccess || VisibleUnits.Contains(r.Recall.UnitId));
        modelBuilder.Entity<Response>().HasQueryFilter(r => SystemAccess || VisibleUnits.Contains(r.Recall.UnitId));
        modelBuilder.Entity<RosterContact>().HasQueryFilter(r => SystemAccess || VisibleUnits.Contains(r.Roster!.UnitId));
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
