using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recall_roster.DTOs;
using recall_roster.Models;

namespace recall_roster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ContactController(ILogger<ContactController> logger, IContactService contacts) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Contact>> GetContacts() => Ok(contacts.GetAllContacts());

    [HttpGet("{id}")]
    public ActionResult<Contact> GetContact(int id)
    {
        var contact = contacts.GetContactById(id);
        return contact == null ? NotFound() : Ok(contact);
    }

    [HttpGet("bynumber/{number}")]
    public ActionResult<Contact> GetContactByNumber(string number)
    {
        var contact = contacts.GetContactByNumber(number);
        return contact == null ? NotFound() : Ok(contact);
    }

    [HttpPost]
    public ActionResult<Contact> AddContact(ContactCreateDto input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var contact = contacts.AddContact(input);
            return CreatedAtAction(nameof(GetContact), new { id = contact.contactId }, contact);
        }
        catch (ArgumentException error) { return BadRequest(new { message = error.Message }); }
        catch (Exception error)
        {
            logger.LogError(error, "Error adding contact");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("remove/{id}")]
    public ActionResult<Contact> RemoveContact(int id)
    {
        var contact = contacts.GetContactById(id);
        if (contact == null) return NotFound();
        contacts.RemoveContact(contact);
        return Ok(contact);
    }

    [HttpPut("{id}")]
    public ActionResult<Contact> UpdateContact(int id, Contact contact)
    {
        if (id != contact.contactId) return BadRequest(new { message = "The route and contact IDs must match." });
        try { return Ok(contacts.UpdateContact(contact)); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (ArgumentException error) { return BadRequest(new { message = error.Message }); }
    }
}
