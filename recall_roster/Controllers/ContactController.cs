using recall_roster.Models;
using recall_roster.DTOs;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Data;


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

    [HttpGet]
    public ActionResult<IEnumerable<Contact>> GetContacts()
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contacts = _contactService.GetAllContacts();
        return Ok(contacts);
    }


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

 [HttpPut("{id}")] 
 
public void UpdateContact(Contact contact)
{
    var existingContact = _dbContext.Contacts.Find(contact.contactId);
    if (existingContact == null)
        throw new ArgumentException("Contact not found.");

    // Update all editable fields
    existingContact.FirstName = contact.FirstName;
    existingContact.LastName = contact.LastName;
    existingContact.PhoneNumber = contact.PhoneNumber;
    existingContact.Rank = contact.Rank;   // <-- Make sure this line exists
    existingContact.Active = contact.Active;

    _dbContext.SaveChanges();
}
}

