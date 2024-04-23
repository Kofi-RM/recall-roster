using recall_roster.Data;
using recall_roster.Models;


    public class RosterRepositoryService : IRosterRepositoryService
    {
        private readonly AppDbContext _context;

        public RosterRepositoryService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Roster> GetAllRosters()
        {
            return _context.Rosters.ToList();
        }

        public Roster? GetRoster(int id)
        {
            return _context.Rosters.FirstOrDefault(r => r.rosterId == id);
        }

        public void RemoveRoster(Roster roster)
        {
            _context.Rosters.Remove(roster);
            _context.SaveChanges();
        }

        public void AddRoster(Roster roster)
        {
            _context.Rosters.Add(roster);
            _context.SaveChanges();
        }

 public void UpdateRoster(Roster roster)
        {
            var existingRoster = _context.Rosters.FirstOrDefault(c => c.rosterId == roster.rosterId);
            if (existingRoster != null)
            {
                existingRoster.name = roster.name;
                existingRoster.description = roster.description;
        

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Contact not found");
            }

        }
    }

public interface IRosterRepositoryService
{
    List<Roster> GetAllRosters();
    Roster? GetRoster(int id);
    void RemoveRoster(Roster roster);
    void AddRoster(Roster roster);
    void UpdateRoster(Roster roster);
}