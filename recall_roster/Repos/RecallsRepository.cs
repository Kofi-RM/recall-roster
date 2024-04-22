using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace recall_roster.Repos
{
    public class RecallRepository
    {
        private readonly AppDbContext _dbContext;

        public RecallRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<Recall> GetAllRecalls()
        {
            return _dbContext.Recalls.ToList();
        }

        public Recall GetRecall(int id)
        {
            var recall = _dbContext.Recalls.FirstOrDefault(r => r.recallId == id);
            return recall;
        }

        public void AddRecall(Recall recall) {
   _dbContext.Recalls.Add(recall);
    _dbContext.SaveChanges();

        }
    }
}
