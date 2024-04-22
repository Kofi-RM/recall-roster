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
    public class RecallController : ControllerBase
    {
        private readonly ILogger<RecallController> _logger;
        private readonly RecallRepository _recallRepository;

        public RecallController(ILogger<RecallController> logger, RecallRepository recallRepository)
        {
            _logger = logger;
            _recallRepository = recallRepository ?? throw new ArgumentNullException(nameof(recallRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Recall>> GetRecalls()
        {
            _logger.LogInformation("Executing GetRecalls action...");
            var recalls = _recallRepository.GetAllRecalls();
            return Ok(recalls);
        }

        [HttpGet("{id}")]
        public ActionResult<Recall> GetRecall(int id)
        {
            _logger.LogInformation("Executing GetRecall action...");
            var recall = _recallRepository.GetRecall(id);
            if (recall == null)
            {
                return NotFound();
            }
            return Ok(recall);
        }

        [HttpPost]
        public ActionResult<Recall> AddRecall(Recall recall)
        {
            _logger.LogInformation("Executing AddRecall action...");
            try
            {
                _recallRepository.AddRecall(recall);
                _logger.LogInformation("Recall added successfully");
                return CreatedAtAction(nameof(GetRecall), new { id = recall.recallId }, recall);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding recall");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
