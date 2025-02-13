using Microsoft.AspNetCore.Mvc;

using recall_roster.Models;



namespace recall_roster.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class RosterController : ControllerBase
    {
        private readonly ILogger<RosterController> _logger;
        private readonly IRosterRepositoryService _rosterRepository;

        public RosterController(ILogger<RosterController> logger, IRosterRepositoryService rosterRepository)
        {
            _logger = logger;
            _rosterRepository = rosterRepository ?? throw new ArgumentNullException(nameof(rosterRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Roster>> GetRosters()
        {
            _logger.LogInformation("Executing GetRosters action...");
            var rosters = _rosterRepository.GetAllRosters();
            return Ok(rosters);
        }

        [HttpGet("{id}")]
        public ActionResult<Roster> GetRoster(int id)
        {
            _logger.LogInformation("Executing GetRoster action...");
            var roster = _rosterRepository.GetRoster(id);
            if (roster == null)
            {
                return NotFound();
            }
            return Ok(roster);
        }

        [HttpPost]
        public ActionResult<Roster> AddRoster(Roster roster)
        {
            _logger.LogInformation("Executing AddRoster action...");
            try
            {
                _rosterRepository.AddRoster(roster);
                _logger.LogInformation("Roster added successfully");
                return CreatedAtAction(nameof(GetRoster), new { id = roster.rosterId }, roster);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding roster");
                return StatusCode(500, "Internal server error");
            }
        }

    

        [HttpDelete("remove/{id}")]
        public ActionResult<Roster> RemoveRoster(int id)
        {
            _logger.LogInformation("Executing RemoveRoster action...");
            var roster = _rosterRepository.GetRoster(id);
            if (roster != null)
            {
                _rosterRepository.RemoveRoster(roster);
                _logger.LogInformation("Roster removed successfully");
                return Ok(roster);
            }
            else
            {
                return NotFound();
            }
        
        }

        [HttpPut("{id}")] 
 
 public ActionResult<Contact> UpdateContact(Roster roster){

 try
            {
                _rosterRepository.UpdateRoster(roster);
                return Ok(roster);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the contact.");
            }
 }
}

    

