using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using recall_roster.Data;
using recall_roster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace recall_roster.Repos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly ILogger<ResponseController> _logger;
        private readonly ResponseRepository _responseRepository;

        public ResponseController(ILogger<ResponseController> logger, ResponseRepository responseRepository)
        {
            _logger = logger;
            _responseRepository = responseRepository ?? throw new ArgumentNullException(nameof(responseRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Response>> GetResponses()
        {
            _logger.LogInformation("Executing GetResponses action...");
            var responses = _responseRepository.GetAllResponses();
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetResponse(int id)
        {
            _logger.LogInformation("Executing GetResponse action...");
            var response = _responseRepository.GetResponse(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        public ActionResult<Response> AddResponse(Response response)
        {
            _logger.LogInformation("Executing AddResponse action...");
            try
            {
                _responseRepository.AddResponse(response);
                _logger.LogInformation("Response added successfully");
                return CreatedAtAction(nameof(GetResponse), new { id = response.responseId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding response");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}