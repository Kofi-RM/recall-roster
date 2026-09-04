using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recall_roster.Models;

namespace recall_roster.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ResponseController : ControllerBase
{
    private readonly IResponseService _service;
    public ResponseController(IResponseService service) => _service = service;
    [HttpGet]
    public ActionResult<IEnumerable<Response>> GetResponses() => Ok(_service.GetAllResponses());
    [HttpGet("{recallId}/{contactId}")]
    public ActionResult<Response> GetResponse(int contactId, int recallId)
    {
        var response = _service.GetResponse(contactId, recallId);
        return response == null ? NotFound() : Ok(response);
    }
    // Acknowledgments may only be written by the signature-validated SMS webhook.
}
