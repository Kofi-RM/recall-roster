using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using recall_roster.Models;
using recall_roster.Repos;
using recall_roster.Data;
using System;
using System.Collections.Generic;

namespace recall_roster.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class RosterController : ControllerBase
    {
        private readonly ILogger<RosterController> _logger;
        private readonly RosterRepository _rosterRepository;

        public RosterController(ILogger<RosterController> logger, RosterRepository rosterRepository)
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

        [HttpPut("update/{id}")]
        public ActionResult<Roster> UpdateRoster(int id, Roster roster)
        {
            _logger.LogInformation("Executing UpdateRoster action...");
            try
            {
                var existingRoster = _rosterRepository.GetRoster(id);
                if (existingRoster == null)
                {
                    return NotFound();
                }
                roster.rosterId = id; // Ensure the ID is set
                _rosterRepository.UpdateRoster(roster);
                _logger.LogInformation("Roster updated successfully");
                return Ok(roster);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roster");
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
    }

