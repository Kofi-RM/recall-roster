using recall_roster.Models;
using recall_roster.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace recall_roster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly IContactService _contactService;

    public ContactController(ILogger<ContactController> logger, IContactService contactService)
    {
        _logger = logger;
        _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
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
        return CreatedAtAction(nameof(GetContact), new { id = contact.contactID }, contact);
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
 
 public ActionResult<Contact> UpdateContact(Contact contact){

 try
            {
                _contactService.UpdateContact(contact);
                return Ok(contact);
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

