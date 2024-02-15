// Controllers/ContactController.cs

using recall_roster.Data;
using recall_roster.Models;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Repos;

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


    [HttpGet("{name}")]
    public ActionResult<Contact> GetContact(string name)
    {
        _logger.LogInformation("Executing GetContacts action...");
        var contact = _contactRepository.GetContact(name);
        if (contact == null)
        {
            return NotFound();
        }
        return Ok(contact);
    }


[HttpGet("remove/{name}")]
public ActionResult<Contact> RemoveContact(string name)
{
    _logger.LogInformation("Executing GetContacts action...");
    var contact = _contactRepository.GetContact(name);
    if (contact != null)
    {
        this._contactRepository.RemoveContact(contact);
        Console.WriteLine("Contact removed");
    } else {
        return NotFound();
    }
    return Ok(contact);
}


}