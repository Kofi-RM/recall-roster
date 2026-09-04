using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recall_roster.DTOs;
using recall_roster.Models;

namespace recall_roster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RecallController : ControllerBase
{
    private readonly IRecallResultsService _service;
    public RecallController(IRecallResultsService service) => _service = service;

    [HttpGet]
    public ActionResult<IEnumerable<Recall>> GetRecalls() => Ok(_service.GetAllRecalls());

    [HttpGet("{id}")]
    public ActionResult<Recall> GetRecall(int id)
    {
        var recall = _service.GetRecall(id);
        return recall == null ? NotFound() : Ok(recall);
    }

    [HttpGet("{id}/recipients")]
    public IActionResult GetRecipients(int id)
    {
        var recall = _service.GetRecall(id);
        if (recall == null) return NotFound();
        return Ok(new { hasRecipientSnapshot = recall.HasRecipientSnapshot, recipients = _service.GetRecipients(id) });
    }

    [HttpPost]
    public ActionResult<Recall> AddRecall(CreateRecallRequest request)
    {
        try
        {
            var recall = _service.AddRecall(request);
            return CreatedAtAction(nameof(GetRecall), new { id = recall.recallId }, recall);
        }
        catch (ArgumentException error) { return BadRequest(new { message = error.Message }); }
    }
}
