using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;

namespace recall_roster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RosterContactController : ControllerBase
    {
        private readonly ILogger<RosterContactController> _logger;
        private readonly RosterContactRepositoryService _rosterRepository;

        public RosterContactController(ILogger<RosterContactController> logger, RosterContactRepositoryService rosterRepository)
        {
            _logger = logger;
            _rosterRepository = rosterRepository ?? throw new ArgumentNullException(nameof(rosterRepository));
        }

    [HttpGet("{id}")]
        public ActionResult<IEnumerable<RosterContact>> GetRosterContacts(int id)
        {
            _logger.LogInformation("Executing GetRosterContacts action...");
            var rosters = _rosterRepository.GetAllRosterContacts(id);
            return Ok(rosters);
        }

        [HttpGet("{rosterId}/{contactId}")]
        public ActionResult<RosterContact> GetRosterContact(int rosterId, int contactId)
        {
            _logger.LogInformation("Executing GetRosterContact action...");
            var roster = _rosterRepository.GetRosterContact(rosterId, contactId);
            if (roster == null)
            {
                return NotFound();
            }
            return Ok(roster);
        }

        [HttpPost]
        public ActionResult<RosterContact> AddRosterContact(RosterContact roster)
        {
            _logger.LogInformation("Executing AddRosterContact action...");
            try
            {
                _rosterRepository.AddRosterContact(roster);
                _logger.LogInformation("Roster_Contact added successfully");
               return CreatedAtAction(nameof(GetRosterContact), new { id = roster.rosterId, id2 = roster.contactId }, roster);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding roster");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("remove/{id}")]
        public ActionResult<RosterContact> RemoveRosterContact(int rosterId, int contactId)
        {
            _logger.LogInformation("Executing RemoveRosterContact action...");
            var roster = _rosterRepository.GetRosterContact(rosterId, contactId);
            if (roster != null)
            {
                _rosterRepository.RemoveRosterContact(roster);
                _logger.LogInformation("Roster_Contact removed successfully");
                return Ok(roster);
            }
            else
            {
                return NotFound();
            }
        }
    

        [HttpPost("updateContacts")]
public IActionResult UpdateRosterContacts([FromBody] UpdateRosterContacts request)
{
    try
    {
        _rosterRepository.UpdateRosterContacts(request.RosterId, request.ContactsToAdd, request.ContactsToRemove);
        _logger.LogInformation("Roster contacts updated successfully");
        return Ok();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating roster contacts");
        return StatusCode(500, "Internal server error");
    }
}
    }
}
