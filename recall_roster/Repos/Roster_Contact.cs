// using recall_roster.Data;
// using recall_roster.Models;
// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace recall_roster.Repos
// {
//     public class Roster_ContactRepository
//     {
//         private readonly AppDbContext _dbContext;

// #pragma warning disable IDE0290 // Use primary constructor
//         public RosterRepository(AppDbContext dbContext)
// #pragma warning restore IDE0290 // Use primary constructor
//         {
//             _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
//         }

//         public List<Roster> GetAllRosters()
//         {
//             return _dbContext.Rosters.ToList();
//         }

//         public Roster GetRoster(int id)
//         {
//             return _dbContext.Rosters.FirstOrDefault(r => r.rosterId == id);
//         }

//         public void RemoveRoster(Roster roster)
//         {
//             _dbContext.Rosters.Remove(roster);
//             _dbContext.SaveChanges();
//         }

//         public void AddRoster(Roster roster)
//         {
//             _dbContext.Rosters.Add(roster);
//             _dbContext.SaveChanges();
//         }

//         // Add other methods for CRUD operations as needed...
//     }
// }