using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;
using recall_roster.Repos;
using System.Collections.Generic;


namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly ResponseRepository _repository;

        public ResponseController(ResponseRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Response>> GetResponses()
        {
            var responses = _repository.GetAllResponses();
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetResponse(int id)
        {
            var response = _repository.GetResponse(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
