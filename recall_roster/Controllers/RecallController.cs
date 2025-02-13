using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;

namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecallController : ControllerBase
    {
        private readonly ILogger<RecallController> _logger;
        private readonly IRecallResultsService _recallResultsService;

        public RecallController(ILogger<RecallController> logger, IRecallResultsService recallResultsService)
        {
            _logger = logger;
            _recallResultsService = recallResultsService ?? throw new ArgumentNullException(nameof(recallResultsService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Recall>> GetRecalls()
        {
            _logger.LogInformation("Executing GetRecalls action...");
            var recalls = _recallResultsService.GetAllRecalls();
            return Ok(recalls);
        }

        [HttpGet("{id}")]
        public ActionResult<Recall> GetRecall(int id)
        {
            _logger.LogInformation("Executing GetRecall action...");
            var recall = _recallResultsService.GetRecall(id);
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
                _recallResultsService.AddRecall(recall);
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
