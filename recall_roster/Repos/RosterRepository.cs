using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace recall_roster.Repos
{
    public class RosterRepository
    {
        private readonly AppDbContext _dbContext;

        public RosterRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<Roster> GetAllRosters()
        {
            return _dbContext.Rosters.ToList();
        }

        public Roster GetRoster(int id)
        {
            return _dbContext.Rosters.FirstOrDefault(r => r.rosterId == id);
        }

        public void RemoveRoster(Roster roster)
        {
            _dbContext.Rosters.Remove(roster);
            _dbContext.SaveChanges();
        }

        public void AddRoster(Roster roster)
        {
            _dbContext.Rosters.Add(roster);
            _dbContext.SaveChanges();
        }

        // Add other methods for CRUD operations as needed...
    }
}
