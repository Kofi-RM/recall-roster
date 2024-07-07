using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;

namespace recall_roster.Repos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly ILogger<ResponseController> _logger;
        private readonly IResponseService _responseService;

        public ResponseController(ILogger<ResponseController> logger, IResponseService responseService)
        {
            _logger = logger;
            _responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Response>> GetResponses()
        {
            _logger.LogInformation("Executing GetResponses action...");
            var responses = _responseService.GetAllResponses();
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetResponse(int id)
        {
            _logger.LogInformation("Executing GetResponse action...");
            var response = _responseService.GetResponse(id);
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
                _responseService.AddResponse(response);
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