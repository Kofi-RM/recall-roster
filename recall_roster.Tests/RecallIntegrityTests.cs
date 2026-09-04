using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using recall_roster.Controllers;
using recall_roster.Data;
using recall_roster.DTOs;
using recall_roster.Models;
using recall_roster.Services;
using Xunit;

public class RecallIntegrityTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly AppDbContext _db;
    public RecallIntegrityTests()
    {
        _connection.Open();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options,
            new ConfigurationBuilder().Build());
        _db.Database.EnsureCreated();
        _db.Contacts.Add(new Contact { contactId = 1, FirstName = "First", LastName = "Person", PhoneNumber = "2025550100", Rank = "Employee", Active = 1 });
        _db.Contacts.Add(new Contact { contactId = 2, FirstName = "Other", LastName = "Person", PhoneNumber = "2025550101", Rank = "Flight Chief", Active = 1 });
        _db.Rosters.Add(new Roster { rosterId = 1, name = "Team", description = "" });
        _db.RosterContacts.Add(new RosterContact { rosterId = 1, contactId = 1 });
        _db.SaveChanges();
    }
    private Recall CreateRecall() => new RecallResultsService(_db).AddRecall(new CreateRecallRequest
    {
        rosterId = 1, message = "Report in", timeEnded = DateTimeOffset.UtcNow.AddHours(1)
    });

    [Fact]
    public void Snapshot_survives_roster_edits_deletion_and_contact_changes()
    {
        var recall = CreateRecall();
        var original = _db.Contacts.Find(1)!;
        original.FirstName = "Changed";
        original.Rank = "Flight Chief";
        original.PhoneNumber = "2025550199";
        _db.Rosters.Remove(_db.Rosters.Find(1)!);
        _db.SaveChanges();
        var recipient = Assert.Single(new RecallResultsService(_db).GetRecipients(recall.recallId));
        Assert.Equal("First", recipient.firstName);
        Assert.Equal("Employee", recipient.rank);
        // The snapshotted phone remains the authorized sender even after contact edits.
        new ResponseService(_db).AddResponse("+12025550100", "1 " + recall.recallId, recall.recallId, 1);
        Assert.True(Assert.Single(new RecallResultsService(_db).GetRecipients(recall.recallId)).responded);
    }

    [Fact]
    public void Duplicate_reply_keeps_first_acknowledgment()
    {
        var recall = CreateRecall();
        var service = new ResponseService(_db);
        service.AddResponse("+12025550100", "first", recall.recallId, 1);
        var first = service.GetResponse(1, recall.recallId)!;
        service.AddResponse("2025550100", "retry", recall.recallId, 1);
        Assert.Single(_db.Responses);
        Assert.Equal(first.responseTime, service.GetResponse(1, recall.recallId)!.responseTime);
        Assert.Equal("first", service.GetResponse(1, recall.recallId)!.response);
        var totals = _db.Recalls.AsNoTracking().Single(r => r.recallId == recall.recallId);
        Assert.Equal(1, totals.Employees);
        Assert.Equal(1, totals.Total);
    }

    [Fact]
    public void Database_rejects_second_canonical_response_but_retains_archived_duplicates()
    {
        var recall = CreateRecall();
        new ResponseService(_db).AddResponse("2025550100", "first", recall.recallId, 1);
        var duplicate = new Response { recallId = recall.recallId, contactId = 1, response = "second", responseTime = DateTime.UtcNow };
        _db.Responses.Add(duplicate);
        Assert.Throws<DbUpdateException>(() => _db.SaveChanges());
        _db.Entry(duplicate).State = EntityState.Detached;
        duplicate.IsDuplicate = true;
        _db.Responses.Add(duplicate);
        _db.SaveChanges();
        Assert.Equal(2, _db.Responses.Count());
        Assert.Single(new ResponseService(_db).GetAllResponses());
    }

    [Fact]
    public void Wrong_sender_nonmember_and_legacy_recall_are_rejected()
    {
        var recall = CreateRecall();
        var service = new ResponseService(_db);
        Assert.Throws<ArgumentException>(() => service.AddResponse("2025550101", "reply", recall.recallId, 1));
        Assert.Throws<ArgumentException>(() => service.AddResponse("2025550101", "reply", recall.recallId, 2));
        recall.HasRecipientSnapshot = false;
        _db.SaveChanges();
        Assert.Throws<ArgumentException>(() => service.AddResponse("2025550100", "reply", recall.recallId, 1));
        Assert.Empty(_db.Responses);
    }

    [Fact]
    public void Legacy_history_does_not_invent_recipients()
    {
        var recall = CreateRecall();
        recall.HasRecipientSnapshot = false;
        _db.SaveChanges();
        Assert.Empty(new RecallResultsService(_db).GetRecipients(recall.recallId));
    }

    [Fact]
    public void Roster_updates_are_idempotent_and_invalid_updates_are_atomic()
    {
        var service = new RosterContactService(_db);
        service.UpdateRosterContacts(1, new[] { 1, 1, 2 }, Array.Empty<int>());
        service.UpdateRosterContacts(1, new[] { 1, 2 }, Array.Empty<int>());
        Assert.Equal(2, _db.RosterContacts.Count());
        Assert.Throws<ArgumentException>(() => service.UpdateRosterContacts(1, new[] { 999 }, new[] { 1 }));
        Assert.Equal(2, _db.RosterContacts.Count());
        Assert.Throws<ArgumentException>(() => service.UpdateRosterContacts(1, new[] { 1 }, new[] { 1 }));
        Assert.Equal(2, _db.RosterContacts.Count());
    }

    [Fact]
    public void Inactive_contacts_are_not_snapshotted_and_deadlines_are_validated()
    {
        _db.Contacts.Find(1)!.Active = 0;
        _db.SaveChanges();
        Assert.Throws<ArgumentException>(() => CreateRecall());
        Assert.Empty(_db.Recalls);
        Assert.Throws<ArgumentException>(() => new RecallResultsService(_db).AddRecall(new CreateRecallRequest
        {
            rosterId = 1, message = "test", timeEnded = DateTimeOffset.UtcNow.AddMinutes(-1)
        }));
        Assert.Empty(_db.RecallRecipients);
    }

    [Theory]
    [InlineData("Unsupported rank", "2025550100")]
    [InlineData("Employee", "not-a-number")]
    public void Invalid_recipient_details_prevent_partial_recall_creation(string rank, string phone)
    {
        var contact = _db.Contacts.Find(1)!;
        contact.Rank = rank;
        contact.PhoneNumber = phone;
        _db.SaveChanges();
        Assert.Throws<ArgumentException>(() => CreateRecall());
        Assert.Empty(_db.Recalls);
        Assert.Empty(_db.RecallRecipients);
    }

    [Fact]
    public void Counter_failure_rolls_back_the_acknowledgment()
    {
        var recall = CreateRecall();
        _db.Database.ExecuteSqlRaw("CREATE TRIGGER reject_counter BEFORE UPDATE ON Recalls BEGIN SELECT RAISE(ABORT, 'test failure'); END;");
        Assert.ThrowsAny<Exception>(() => new ResponseService(_db).AddResponse("2025550100", "reply", recall.recallId, 1));
        Assert.Empty(_db.Responses.AsNoTracking());
        Assert.Equal(0, _db.Recalls.AsNoTracking().Single().Total);
    }

    [Theory]
    [InlineData("1 2", true)]
    [InlineData("1,2", true)]
    [InlineData("invalid 2", false)]
    [InlineData("0 2", false)]
    [InlineData("1 -2", false)]
    [InlineData("1 2 3", false)]
    public void Sms_ids_must_be_positive_integers(string input, bool expected) =>
        Assert.Equal(expected, MessageController.TryReadIds(input, out _, out _));

    [Fact]
    public async Task Webhook_rejects_unsigned_and_tampered_requests_and_accepts_valid_signature()
    {
        var recall = CreateRecall();
        const string token = "test-auth-token-not-a-secret";
        const string url = "https://example.test/api/Message/ReceiveMessage";
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Twilio:AuthToken"] = token, ["Twilio:WebhookUrl"] = url
        }).Build();
        var http = new DefaultHttpContext();
        http.Request.ContentType = "application/x-www-form-urlencoded";
        var fields = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            ["From"] = "+12025550100", ["Body"] = "1 " + recall.recallId
        };
        http.Request.Form = new FormCollection(fields);
        var controller = new MessageController(new NoMessages(), new ResponseService(_db), config)
        { ControllerContext = new ControllerContext { HttpContext = http } };
        Assert.Equal(403, Assert.IsType<StatusCodeResult>(await controller.ReceiveSms()).StatusCode);
        http.Request.Headers["X-Twilio-Signature"] = "invalid";
        Assert.Equal(403, Assert.IsType<StatusCodeResult>(await controller.ReceiveSms()).StatusCode);
        Assert.Empty(_db.Responses);
        var signed = url + string.Concat(fields.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => p.Key + p.Value.ToString()));
        http.Request.Headers["X-Twilio-Signature"] = Convert.ToBase64String(
            HMACSHA1.HashData(Encoding.UTF8.GetBytes(token), Encoding.UTF8.GetBytes(signed)));
        await controller.ReceiveSms();
        Assert.Single(_db.Responses);
        fields["Body"] = "2 " + recall.recallId;
        http.Request.Form = new FormCollection(fields);
        Assert.Equal(403, Assert.IsType<StatusCodeResult>(await controller.ReceiveSms()).StatusCode);
    }

    [Fact]
    public void Response_api_requires_authentication_and_has_no_manual_write_action()
    {
        Assert.NotNull(typeof(ResponseController).GetCustomAttribute<AuthorizeAttribute>());
        Assert.DoesNotContain(typeof(ResponseController).GetMethods(), method => method.GetCustomAttribute<HttpPostAttribute>() != null);
        Assert.NotNull(typeof(MessageController).GetMethod("SendMessage")!.GetCustomAttribute<AuthorizeAttribute>());
    }

    [Fact]
    public void Contact_edit_rejects_mismatched_ids_without_changing_either_contact()
    {
        var controller = new ContactController(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<ContactController>.Instance,
            new ContactService(_db), _db);
        var input = new Contact { contactId=2, FirstName="Changed", LastName="Name", PhoneNumber="2025550100", Rank="Employee" };
        Assert.IsType<BadRequestObjectResult>(controller.UpdateContact(1, input).Result);
        Assert.Equal("First", _db.Contacts.Find(1)!.FirstName);
        Assert.Equal("Other", _db.Contacts.Find(2)!.FirstName);
    }

    [Fact]
    public void Contact_edit_preserves_activation_and_validates_before_saving()
    {
        var controller = new ContactController(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<ContactController>.Instance,
            new ContactService(_db), _db);
        var input = new Contact { contactId=1, FirstName=" Updated ", LastName=" Person ", PhoneNumber="2025550100", Rank="Flight Chief", Active=0 };
        Assert.IsType<OkObjectResult>(controller.UpdateContact(1, input).Result);
        Assert.Equal(1, _db.Contacts.Find(1)!.Active);
        Assert.Equal("Updated", _db.Contacts.Find(1)!.FirstName);
        input.FirstName="Should not persist";
        input.PhoneNumber="invalid";
        Assert.IsType<BadRequestObjectResult>(controller.UpdateContact(1, input).Result);
        Assert.Equal("Updated", _db.Contacts.Find(1)!.FirstName);
    }

    [Fact]
    public void Roster_update_requires_auth_and_matching_route_id()
    {
        Assert.NotNull(typeof(RosterController).GetMethod("UpdateContact")!.GetCustomAttribute<AuthorizeAttribute>());
        var controller = new RosterController(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<RosterController>.Instance,
            new RosterRepositoryService(_db));
        Assert.IsType<BadRequestObjectResult>(controller.UpdateContact(1,
            new Roster { rosterId=2, name="Wrong roster", description="" }).Result);
        Assert.Equal("Team", _db.Rosters.Find(1)!.name);
    }

    private class NoMessages : IMessageService
    {
        public void SendMessage(string recipient, string body) => throw new InvalidOperationException("Tests must not send SMS.");
        public void SendMessageByID(int contactId, int recallId, string body) => throw new InvalidOperationException("Tests must not send SMS.");
    }
    public void Dispose() { _db.Dispose(); _connection.Dispose(); }
}
