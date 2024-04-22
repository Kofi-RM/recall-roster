using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace recall_roster.Repos
{
    public class ResponseRepository
    {
        private readonly AppDbContext _dbContext;

        public ResponseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<Response> GetAllResponses()
        {
            return _dbContext.Responses.ToList();
        }

        public Response GetResponse(int id)
        {
            var response = _dbContext.Responses.FirstOrDefault(r => r.responseId == id);
            return response;
        }

         public void AddResponse(Response response)
        {
            _dbContext.Responses.Add(response);
            _dbContext.SaveChanges();
        }
    }
}
