using recall_roster.Models;
using recall_roster.DTOs;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Data;
using Microsoft.AspNetCore.Authorization;


namespace recall_roster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly IContactService _contactService;

    private readonly AppDbContext _dbContext;
    

    public ContactController(ILogger<ContactController> logger, IContactService contactService, AppDbContext dbContext)
    {
        _logger = logger;
        _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    [Authorize]
    [HttpGet]
    public ActionResult<IEnumerable<Contact>> GetContacts()
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contacts = _contactService.GetAllContacts();
        return Ok(contacts);
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<Contact> GetContact(int id)
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contact = _contactService.GetContactById(id);
        if (contact == null)
        {
            return NotFound();
        }
        return Ok(contact);
    }
[Authorize]
[HttpGet("bynumber/{number}")]
    public ActionResult<Contact> GetContactByNumber(string number)
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contact = _contactService.GetContactByNumber(number);
        if (contact == null)
        {
            return NotFound();
        }
        return Ok(contact);
    }
[Authorize]
[HttpPost]
public ActionResult<ContactCreateDto> AddContact(ContactCreateDto contact)
{
    _logger.LogInformation("Executing AddContact action...");
     {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
    {
        _contactService.AddContact(contact);
        _logger.LogInformation("Contact added successfully");
        return CreatedAtAction(nameof(GetContact), new { id = contact.contactId }, contact);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding contact");
        return StatusCode(500, "Internal server error");
    }
}
}
[Authorize]
[HttpPut("remove/{id}")]
public ActionResult<Contact> RemoveContact(int id)
{
    _logger.LogInformation("Executing GetContacts action...");
    var contact = _contactService.GetContactById(id);
    if (contact != null)
    {
        this._contactService.RemoveContact(contact);
        Console.WriteLine("Contact removed");
    } else {
        return NotFound();
    }
    return Ok(contact);
}
[Authorize]
 [HttpPut("{id}")] 
 
public ActionResult<Contact> UpdateContact(int id, Contact contact)
{
    if (id != contact.contactId) return BadRequest(new { message = "The route and contact IDs must match." });
    var existingContact = _dbContext.Contacts.Find(id);
    if (existingContact == null)
        return NotFound();
    if (string.IsNullOrWhiteSpace(contact.FirstName) || string.IsNullOrWhiteSpace(contact.LastName) ||
        !new[] { "Employee", "Element Chief", "Flight Chief", "Squadron Director" }.Contains(contact.Rank))
        return BadRequest(new { message = "First name, last name, and a supported staff rank are required." });
    var phone = contact.PhoneNumber?.Trim() ?? "";
    try { recall_roster.Services.PhoneNumbers.Normalize(phone); }
    catch (ArgumentException) { return BadRequest(new { message = "Enter a valid phone number." }); }

    // Update all editable fields
    existingContact.FirstName = contact.FirstName.Trim();
    existingContact.LastName = contact.LastName.Trim();
    existingContact.PhoneNumber = phone;
    existingContact.Rank = contact.Rank;
    // Activation changes belong to the separate removal workflow, not an ordinary edit.

    _dbContext.SaveChanges();
    return Ok(existingContact);
}
}

