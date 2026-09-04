using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
using recall_roster.Services;

public class ResponseService : IResponseService
{
    private readonly AppDbContext _context;
    public ResponseService(AppDbContext context) => _context = context;

    public List<Response> GetAllResponses() => _context.Responses.AsNoTracking()
        .Where(r => !r.IsDuplicate).OrderBy(r => r.responseTime).ThenBy(r => r.responseId).ToList();

    public Response? GetResponse(int contactId, int recallId) => _context.Responses.AsNoTracking()
        .Where(r => r.contactId == contactId && r.recallId == recallId && !r.IsDuplicate)
        .OrderBy(r => r.responseTime).ThenBy(r => r.responseId).FirstOrDefault();

    public void AddResponse(string from, string body, int recallId, int contactId)
    {
        if (string.IsNullOrWhiteSpace(body) || body.Length > 1600)
            throw new ArgumentException("A response of at most 1600 characters is required.");
        var recall = _context.Recalls.AsNoTracking().SingleOrDefault(r => r.recallId == recallId);
        if (recall == null || !recall.HasRecipientSnapshot)
            throw new ArgumentException("This recall does not have a verified recipient snapshot.");
        var recipient = _context.RecallRecipients.AsNoTracking()
            .SingleOrDefault(r => r.recallId == recallId && r.contactId == contactId);
        if (recipient == null || PhoneNumbers.Normalize(recipient.PhoneNumber) != PhoneNumbers.Normalize(from))
            throw new ArgumentException("The sender is not a recipient of this recall.");
        // Late acknowledgments are intentionally accepted and labeled late by the UI.
        if (GetResponse(contactId, recallId) != null) return;
        var response = new Response
        {
            contactId = contactId, recallId = recallId,
            response = body, responseTime = DateTime.UtcNow
        };
        _context.Responses.Add(response);
        try
        {
            using var transaction = _context.Database.BeginTransaction();
            _context.SaveChanges();
            var target = _context.Recalls.Where(r => r.recallId == recallId);
            switch (recipient.Rank)
            {
                case "Employee":
                    target.ExecuteUpdate(s => s.SetProperty(r => r.Employees, r => r.Employees + 1)); break;
                case "Element Chief":
                    target.ExecuteUpdate(s => s.SetProperty(r => r.ElementChief, r => r.ElementChief + 1)); break;
                case "Flight Chief":
                    target.ExecuteUpdate(s => s.SetProperty(r => r.FlightChief, r => r.FlightChief + 1)); break;
                case "Squadron Director":
                    target.ExecuteUpdate(s => s.SetProperty(r => r.SquadronDirector, r => r.SquadronDirector + 1)); break;
            }
            transaction.Commit();
        }
        catch (DbUpdateException)
        {
            _context.Entry(response).State = EntityState.Detached;
            // A simultaneous retry may have won the unique-key race. Other DB failures must surface.
            if (GetResponse(contactId, recallId) == null) throw;
        }
    }
}

public interface IResponseService
{
    List<Response> GetAllResponses();
    Response? GetResponse(int contactId, int recallId);
    void AddResponse(string from, string body, int recallId, int contactId);
}
