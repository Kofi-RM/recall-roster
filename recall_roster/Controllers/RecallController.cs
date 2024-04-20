using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;
using recall_roster.Repos;
using System.Collections.Generic;

namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecallController : ControllerBase
    {
        private readonly RecallRepository _repository;

        public RecallController(RecallRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Recall>> GetRecalls()
        {
            var recalls = _repository.GetAllRecalls();
            return Ok(recalls);
        }

        [HttpGet("{id}")]
        public ActionResult<Recall> GetRecall(int id)
        {
            var recall = _repository.GetRecall(id);
            if (recall == null)
            {
                return NotFound();
            }
            return Ok(recall);
        }
    }
}
