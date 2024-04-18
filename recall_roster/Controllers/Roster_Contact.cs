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
    public class Roster_ContactController : ControllerBase
    {
        private readonly ILogger<Roster_ContactController> _logger;
        private readonly Roster_ContactRepository _rosterRepository;

        public RosterController(ILogger<Roster_ContactController> logger, Roster_ContactRepository rosterRepository)
        {
            _logger = logger;
            _rosterRepository = rosterRepository ?? throw new ArgumentNullException(nameof(rosterRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Roster_Contact>> GetRoster_Contacts()
        {
            _logger.LogInformation("Executing GetRoster_Contacts action...");
            var rosters = _rosterRepository.GetAllRoster_Contacts();
            return Ok(rosters);
        }

        [HttpGet("{id}")]
        public ActionResult<Roster_Contact> GetRoster_Contact(int id)
        {
            _logger.LogInformation("Executing GetRoster_Contact action...");
            var roster = _rosterRepository.GetRoster_Contact(id);
            if (roster == null)
            {
                return NotFound();
            }
            return Ok(roster);
        }

        // [HttpPost]
        // public ActionResult<Roster_Contact> AddRoster_Contact(Roster_Contact roster)
        // {
        //     _logger.LogInformation("Executing AddRoster_Contact action...");
        //     try
        //     {
        //         _rosterRepository.AddRoster_Contact(roster);
        //         _logger.LogInformation("Roster_Contact added successfully");
        //         return CreatedAtAction(nameof(GetRoster_Contact), new { id = roster.rosterID }, roster);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error adding roster");
        //         return StatusCode(500, "Internal server error");
        //     }
        // }

        // [HttpPut("update/{id}")]
        // public ActionResult<Roster_Contact> UpdateRoster_Contact(int id, Roster_Contact roster)
        // {
        //     _logger.LogInformation("Executing UpdateRoster_Contact action...");
        //     try
        //     {
        //         var existingRoster_Contact = _rosterRepository.GetRoster_Contact(id);
        //         if (existingRoster_Contact == null)
        //         {
        //             return NotFound();
        //         }
        //         roster.Roster_ContactID = id; // Ensure the ID is set
        //         _rosterRepository.UpdateRoster_Contact(roster);
        //         _logger.LogInformation("Roster_Contact updated successfully");
        //         return Ok(roster);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error updating roster");
        //         return StatusCode(500, "Internal server error");
        //     }
        // }

        [HttpDelete("remove/{id}")]
        public ActionResult<Roster_Contact> RemoveRoster_Contact(int id)
        {
            _logger.LogInformation("Executing RemoveRoster_Contact action...");
            var roster = _rosterRepository.GetRoster_Contact(id);
            if (roster != null)
            {
                _rosterRepository.RemoveRoster_Contact(roster);
                _logger.LogInformation("Roster_Contact removed successfully");
                return Ok(roster);
            }
            else
            {
                return NotFound();
            }
        }
    }

