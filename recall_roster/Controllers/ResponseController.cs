using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;
using Twilio.TwiML;
using Twilio.AspNet.Core;

namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
            private readonly IContactService _contactService; 
        private readonly ILogger<ResponseController> _logger;
        private readonly IResponseService _responseService;

        public ResponseController(ILogger<ResponseController> logger, IResponseService responseService, IContactService contactService)
        {
            _logger = logger;
            _responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
              _contactService = contactService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Response>> GetResponses()
        {
            _logger.LogInformation("Executing GetResponses action...");
            var responses = _responseService.GetAllResponses();
            return Ok(responses);
        }

        [HttpGet("{recallId}/{contactId}")]
        public ActionResult<Response> GetResponse(int contactId, int recallId)
        {
            _logger.LogInformation("Executing GetResponse action...");
            var response = _responseService.GetResponse(contactId, recallId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
public ActionResult<Response> AddResponse( string from, string body, int recallId)
{

        try {
        _responseService.AddResponse(from, body, recallId);
        //_logger.LogInformation("Response added successfully");
         return Ok("Response added successfully.");
    
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding response");
        return StatusCode(500, "Internal server error");
    }
}
    }
}

//     private int ExtractRecallIdFromMessage(string messageBody)
// {
//     // Implement logic to extract the recall ID from the message body
//     // This could involve parsing the message or using regex if your format is consistent
//     // Example:
//     var match = Regex.Match(messageBody, @"Recall ID is: (\d+)");
//     return int.Parse(match.Groups[1].Value);
// }
// }