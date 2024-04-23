using recall_roster.Models;
using recall_roster.Repos;
using Microsoft.AspNetCore.Mvc;


namespace recall_roster.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly ContactRepository _contactRepository;

    public ContactController(ILogger<ContactController> logger, ContactRepository contactRepository)
    {
        _logger = logger;
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
    }

    [HttpGet]
    public ActionResult<IEnumerable<Contact>> GetContacts()
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contacts = _contactRepository.GetAllContacts();
        return Ok(contacts);
    }


    [HttpGet("{id}")]
    public ActionResult<Contact> GetContact(int id)
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contact = _contactRepository.GetContact(id);
        if (contact == null)
        {
            return NotFound();
        }
        return Ok(contact);
    }

[HttpPost]
public ActionResult<Contact> AddContact(Contact contact)
{
    _logger.LogInformation("Executing AddContact action...");
    try
    {
        _contactRepository.AddContact(contact);
        _logger.LogInformation("Contact added successfully");
        return CreatedAtAction(nameof(GetContact), new { id = contact.contactID }, contact);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding contact");
        return StatusCode(500, "Internal server error");
    }
}

[HttpPut("remove/{id}")]
public ActionResult<Contact> RemoveContact(int id)
{
    _logger.LogInformation("Executing GetContacts action...");
    var contact = _contactRepository.GetContact(id);
    if (contact != null)
    {
        this._contactRepository.RemoveContact(contact);
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
                _contactRepository.UpdateContact(contact);
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

