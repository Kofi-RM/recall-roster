using System.Data;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.DTOs;
using recall_roster.Models;
using recall_roster.Services;

public class RecallResultsService : IRecallResultsService
{
    private readonly AppDbContext _context;
    public RecallResultsService(AppDbContext context) => _context = context;

    public List<Recall> GetAllRecalls() => _context.Recalls.AsNoTracking().OrderByDescending(r => r.timeStarted).ToList();
    public Recall? GetRecall(int id) => _context.Recalls.AsNoTracking().FirstOrDefault(r => r.recallId == id);

    public Recall AddRecall(CreateRecallRequest request)
    {
        var now = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(request.message) || request.message.Length > 1600 ||
            request.timeEnded.UtcDateTime <= now)
            throw new ArgumentException("A message and a future deadline are required.");

        using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
        var rosterIds = (request.rosterIds ?? new[] { request.rosterId }).Distinct().OrderBy(id => id).ToArray();
        if (rosterIds.Length == 0 || rosterIds.Any(id => id <= 0))
            throw new ArgumentException("Choose at least one valid roster.");
        var rosters = _context.Rosters.Where(r => rosterIds.Contains(r.rosterId)).ToList();
        if (rosters.Count != rosterIds.Length)
            throw new ArgumentException("One or more rosters were not found.");
        if (rosters.Any(r => !_context.Access.CanInitiate(r.UnitId)))
            throw new UnauthorizedAccessException("Recall initiation permission is required for every roster.");
        if (rosters.Select(r => r.UnitId).Distinct().Count() != 1)
            throw new ArgumentException("Select rosters from the same unit for one recall.");
        var roster = rosters[0];
        var contactIds = _context.RosterContacts.Where(rc => rosterIds.Contains(rc.rosterId)).Select(rc => rc.contactId);
        var contacts = _context.Contacts.Where(c => contactIds.Contains(c.contactId) && c.Active == 1 && c.UnitId == roster.UnitId)
            .AsNoTracking().ToList();
        if (contacts.Count == 0) throw new ArgumentException("The roster has no active recipients.");
        if (contacts.Any(c => !StaffRanks.IsSupported(c.Rank)))
            throw new ArgumentException("Every recipient must have a supported staff rank before starting a recall.");

        var recall = new Recall
        {
            rosterId = rosterIds[0],
            RosterIdsJson = System.Text.Json.JsonSerializer.Serialize(rosterIds),
            UnitId = roster.UnitId,
            CreatedByUserId = _context.Access.UserId,
            message = request.message.Trim(),
            timeStarted = now,
            timeEnded = request.timeEnded.UtcDateTime,
            HasRecipientSnapshot = true,
            EmployeesMax = contacts.Count(c => c.Rank == "Employee"),
            ElementChiefMax = contacts.Count(c => c.Rank == "Element Chief"),
            FlightChiefMax = contacts.Count(c => c.Rank == "Flight Chief"),
            SquadronDirectorMax = contacts.Count(c => c.Rank == "Squadron Director"),
            TotalMax = contacts.Count,
            Recipients = contacts.Select(c => new RecallRecipient
            {
                contactId = c.contactId, FirstName = c.FirstName, LastName = c.LastName,
                PhoneNumber = PhoneNumbers.Normalize(c.PhoneNumber), Rank = c.Rank
            }).ToList()
        };
        _context.Recalls.Add(recall);
        _context.SaveChanges();
        transaction.Commit();
        return recall;
    }

    public List<RecipientStatus> GetRecipients(int recallId)
    {
        var recall = GetRecall(recallId) ?? throw new ArgumentException("Recall not found.");
        var replies = _context.Responses.AsNoTracking()
            .Where(r => r.recallId == recallId && !r.IsDuplicate)
            .OrderBy(r => r.responseTime).ThenBy(r => r.responseId).ToList()
            .GroupBy(r => r.contactId).ToDictionary(g => g.Key, g => g.First().responseTime);
        if (!recall.HasRecipientSnapshot)
        {
            // Never invent a historical denominator or historical names from current contacts.
            return replies.Select(r => new RecipientStatus(r.Key, $"Contact #{r.Key}", "", "Unknown", true,
                DateTime.SpecifyKind(r.Value, DateTimeKind.Utc))).ToList();
        }
        return _context.RecallRecipients.AsNoTracking().Where(r => r.recallId == recallId)
            .OrderBy(r => r.LastName).ThenBy(r => r.FirstName).ToList()
            .Select(r => new RecipientStatus(r.contactId, r.FirstName, r.LastName, r.Rank,
                replies.ContainsKey(r.contactId), replies.TryGetValue(r.contactId, out var time)
                    ? DateTime.SpecifyKind(time, DateTimeKind.Utc) : null)).ToList();
    }
}

public record RecipientStatus(int contactId, string firstName, string lastName, string rank, bool responded, DateTime? responseTime);
public interface IRecallResultsService
{
    List<Recall> GetAllRecalls();
    Recall? GetRecall(int id);
    Recall AddRecall(CreateRecallRequest request);
    List<RecipientStatus> GetRecipients(int recallId);
}
