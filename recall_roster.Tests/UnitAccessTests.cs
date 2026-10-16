using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using recall_roster.Controllers;
using recall_roster.Data;
using recall_roster.DTOs;
using recall_roster.Models;
using recall_roster.Services;
using Xunit;

public sealed class UnitAccessTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly AppDbContext db;
    private readonly UnitAccessScope scope = new() { IsSystem = true };
    private readonly UnitAccessService access;
    private readonly int flightRecall;
    private readonly int otherRecall;
    public UnitAccessTests()
    {
        connection.Open();
        db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options, new ConfigurationBuilder().Build(), scope);
        db.Database.EnsureCreated();
        db.Units.AddRange(new Unit { Id = 1, Name = "Squadron" }, new Unit { Id = 2, Name = "Flight", ParentUnitId = 1 }, new Unit { Id = 3, Name = "Other squadron" });
        db.Users.AddRange(new User { Id = 1, Email = "LEADER@EXAMPLE.TEST", Password = "hash" }, new User { Id = 2, Email = "COORDINATOR@EXAMPLE.TEST", Password = "hash" }, new User { Id = 3, Email = "ADMIN@EXAMPLE.TEST", Password = "hash", IsSystemAdministrator = true }, new User { Id = 4, Email = "UNASSIGNED@EXAMPLE.TEST", Password = "hash" });
        db.SaveChanges();
        db.Contacts.AddRange(new Contact { contactId = 1, UnitId = 2, FirstName = "Flight", LastName = "Person", PhoneNumber = "2025550100", Rank = "Employee", Active = 1 }, new Contact { contactId = 2, UnitId = 3, FirstName = "Other", LastName = "Person", PhoneNumber = "2025550101", Rank = "Employee", Active = 1 });
        db.Rosters.AddRange(new Roster { rosterId = 1, UnitId = 2, name = "Flight team", description = "" }, new Roster { rosterId = 2, UnitId = 3, name = "Other team", description = "" });
        db.UnitMemberships.AddRange(new UnitMembership { UserId = 1, UnitId = 1, IncludeDescendants = true }, new UnitMembership { UserId = 2, UnitId = 2, CanManageRosters = true, CanInitiateRecalls = true });
        db.SaveChanges();
        db.RosterContacts.AddRange(new RosterContact { rosterId = 1, contactId = 1 }, new RosterContact { rosterId = 2, contactId = 2 });
        db.SaveChanges();
        var recalls = new RecallResultsService(db);
        flightRecall = recalls.AddRecall(new CreateRecallRequest { rosterId = 1, message = "Flight recall", timeEnded = DateTimeOffset.UtcNow.AddHours(1) }).recallId;
        otherRecall = recalls.AddRecall(new CreateRecallRequest { rosterId = 2, message = "Other recall", timeEnded = DateTimeOffset.UtcNow.AddHours(1) }).recallId;
        new ResponseService(db).AddResponse("2025550100", "ack", flightRecall, 1);
        new ResponseService(db).AddResponse("2025550101", "ack", otherRecall, 2);
        db.ChangeTracker.Clear();
        access = new UnitAccessService(db, scope);
    }
    private static ClaimsPrincipal Principal(int id) => new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }, "test"));
    private async Task Load(int id) => Assert.True(await access.Load(Principal(id)));

    [Fact]
    public async Task Leadership_reads_descendants_but_has_no_implicit_write_permissions()
    {
        await Load(1);
        Assert.Equal(new[] { 1, 2 }, scope.VisibleUnitIds.OrderBy(id => id));
        Assert.Empty(scope.ManagedUnitIds); Assert.Empty(scope.RecallUnitIds);
        Assert.Equal("Flight team", Assert.Single(db.Rosters).name);
        Assert.Equal(flightRecall, Assert.Single(db.Recalls).recallId);
        Assert.Equal(1, Assert.Single(db.Contacts).contactId);
        Assert.Single(db.RecallRecipients); Assert.Single(db.Responses); Assert.Single(db.RosterContacts);
        Assert.Null(new RecallResultsService(db).GetRecall(otherRecall));
        Assert.Null(new ContactService(db).GetContactById(2));
    }

    [Fact]
    public async Task No_membership_and_administrator_alone_do_not_grant_operational_access()
    {
        foreach (var id in new[] { 3, 4 })
        {
            await Load(id);
            Assert.Empty(db.Contacts); Assert.Empty(db.Recalls); Assert.Empty(db.Rosters); Assert.Empty(db.Responses);
            Assert.Empty(scope.ManagedUnitIds); Assert.Empty(scope.RecallUnitIds);
        }
    }

    [Fact]
    public async Task Coordinator_permissions_do_not_extend_to_siblings_or_parents()
    {
        await Load(2);
        Assert.Equal(new[] { 2 }, scope.VisibleUnitIds);
        Assert.True(scope.CanManage(2)); Assert.True(scope.CanInitiate(2));
        Assert.False(scope.CanManage(1)); Assert.False(scope.CanManage(3));
    }

    [Fact]
    public async Task Revocation_and_disabling_take_effect_without_reissuing_token()
    {
        var principal = Principal(2);
        Assert.True(await access.Load(principal));
        scope.IsSystem = true;
        db.UnitMemberships.Remove(await db.UnitMemberships.FindAsync(2, 2) ?? throw new Exception());
        await db.SaveChangesAsync();
        Assert.True(await access.Load(principal));
        Assert.Empty(scope.VisibleUnitIds); Assert.Empty(db.Recalls);
        scope.IsSystem = true;
        (await db.Users.FindAsync(2))!.IsEnabled = false;
        await db.SaveChangesAsync();
        Assert.False(await access.Load(principal));
        Assert.False(scope.IsAdministrator); Assert.Empty(scope.VisibleUnitIds);
    }

    [Fact]
    public async Task Read_only_members_cannot_edit_contacts_or_create_recalls()
    {
        await Load(1);
        var contact = new Contact { contactId = 1, UnitId = 2, FirstName = "Changed", LastName = "Person", PhoneNumber = "2025550100", Rank = "Employee" };
        Assert.Throws<UnauthorizedAccessException>(() => new ContactService(db).UpdateContact(contact));
        db.ChangeTracker.Clear();
        Assert.Equal("Flight", db.Contacts.Single().FirstName);
        Assert.Throws<UnauthorizedAccessException>(() => new RecallResultsService(db).AddRecall(new CreateRecallRequest { rosterId = 1, message = "Unauthorized", timeEnded = DateTimeOffset.UtcNow.AddHours(1) }));
        db.ChangeTracker.Clear();
        Assert.Single(db.Recalls);
    }

    [Fact]
    public async Task Cross_unit_membership_is_rejected_even_when_both_units_are_manageable()
    {
        db.UnitMemberships.Add(new UnitMembership { UserId = 2, UnitId = 3, CanManageRosters = true }); db.SaveChanges();
        await Load(2);
        Assert.Throws<ArgumentException>(() => new RosterContactService(db).UpdateRosterContacts(1, new[] { 2 }, Array.Empty<int>()));
        Assert.Equal(1, db.RosterContacts.Count(r => r.rosterId == 1));
    }

    [Fact]
    public async Task Unit_ownership_cannot_be_changed_through_ordinary_edits()
    {
        db.UnitMemberships.Add(new UnitMembership { UserId = 2, UnitId = 3, CanManageRosters = true }); db.SaveChanges();
        await Load(2);
        db.Contacts.Single(c => c.contactId == 1).UnitId = 3;
        Assert.Throws<UnauthorizedAccessException>(() => db.SaveChanges());
    }

    [Fact]
    public async Task New_recall_records_unit_and_creator_and_survives_coordinator_reassignment()
    {
        await Load(2);
        var recall = new RecallResultsService(db).AddRecall(new CreateRecallRequest { rosterId = 1, message = "New", timeEnded = DateTimeOffset.UtcNow.AddHours(1) });
        Assert.Equal(2, recall.UnitId); Assert.Equal(2, recall.CreatedByUserId);
        scope.IsSystem = true;
        db.UnitMemberships.Remove(db.UnitMemberships.Find(2, 2)!); db.SaveChanges();
        await Load(1);
        Assert.NotNull(new RecallResultsService(db).GetRecall(recall.recallId));
    }

    [Fact]
    public async Task Unauthorized_send_never_calls_sms_provider()
    {
        var sender = new NeverSend();
        await Load(1);
        Assert.Throws<UnauthorizedAccessException>(() => new MessageService(db, sender).SendMessageByID(1, flightRecall, "ignored"));
        Assert.Throws<ArgumentException>(() => new MessageService(db, sender).SendMessageByID(2, otherRecall, "ignored"));
    }

    [Fact]
    public async Task Filter_blocks_viewer_writes_guessed_ids_and_account_administration()
    {
        Assert.IsType<ForbidResult>(await Filter(typeof(RecallController), "AddRecall", "POST", 1, new() { ["request"] = new CreateRecallRequest { rosterId = 1 } }));
        Assert.IsType<NotFoundResult>(await Filter(typeof(MessageController), "SendMessage", "POST", 2, new() { ["contactId"] = 2, ["recallId"] = otherRecall }));
        Assert.IsType<ForbidResult>(await Filter(typeof(UserController), "GetAllUsers", "GET", 2, new()));
        Assert.IsType<ForbidResult>(await Filter(typeof(AuthController), "Register", "POST", 2, new() { ["request"] = new RegisterRequest { Email = "test@example.test", Password = "test" } }));
        Assert.IsType<ForbidResult>(await Filter(typeof(AccessController), "Grant", "PUT", 2, new()));
        Assert.IsType<NoContentResult>(await Filter(typeof(AccessController), "Me", "GET", 2, new()));
        Assert.IsType<NoContentResult>(await Filter(typeof(AccessController), "Units", "GET", 3, new()));
    }

    [Fact]
    public async Task Anonymous_or_disabled_user_is_rejected_but_provider_endpoint_still_runs_signature_validation()
    {
        Assert.IsType<UnauthorizedResult>(await Filter(typeof(RecallController), "GetRecalls", "GET", null, new()));
        Assert.IsType<NoContentResult>(await Filter(typeof(MessageController), "ReceiveSms", "POST", null, new()));
        Assert.False(scope.IsSystem);
    }

    [Fact]
    public async Task Administrator_grants_and_revokes_are_audited_and_last_admin_is_protected()
    {
        await Load(3);
        var controller = new AccessController(db, scope);
        Assert.IsType<OkObjectResult>(await controller.Grant(4, 2, new AccessController.MembershipRequest(false, false, false)));
        Assert.Equal("Grant membership", db.AccessAudits.Single().Action);
        Assert.IsType<NoContentResult>(await controller.Revoke(4, 2));
        Assert.Equal(2, db.AccessAudits.Count());
        Assert.IsType<BadRequestObjectResult>(await controller.UpdateUser(3, new AccessController.UserPermissions(false, false)));
        Assert.True(db.Users.Single(u => u.Id == 3).IsSystemAdministrator);
    }

    [Fact]
    public async Task Signed_sms_can_record_reply_even_without_a_dashboard_login()
    {
        // Use a new recall so the response is not an existing duplicate.
        var recall = new RecallResultsService(db).AddRecall(new CreateRecallRequest { rosterId = 1, message = "SMS", timeEnded = DateTimeOffset.UtcNow.AddHours(1) });
        scope.IsSystem = false; scope.VisibleUnitIds = Array.Empty<int>();
        const string token = "test-token";
        const string url = "https://example.test/api/Message/ReceiveMessage";
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Twilio:AuthToken"] = token, ["Twilio:WebhookUrl"] = url }).Build();
        var http = new DefaultHttpContext(); http.Request.ContentType = "application/x-www-form-urlencoded";
        var fields = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { ["From"] = "+12025550100", ["Body"] = "1 " + recall.recallId };
        http.Request.Form = new FormCollection(fields);
        var controller = new MessageController(new MessageService(db, new NeverSend()), new ResponseService(db), config, db)
            { ControllerContext = new ControllerContext { HttpContext = http } };
        Assert.IsType<StatusCodeResult>(await controller.ReceiveSms()); Assert.False(scope.IsSystem);
        var signed = url + string.Concat(fields.OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => pair.Key + pair.Value.ToString()));
        http.Request.Headers["X-Twilio-Signature"] = Convert.ToBase64String(System.Security.Cryptography.HMACSHA1.HashData(System.Text.Encoding.UTF8.GetBytes(token), System.Text.Encoding.UTF8.GetBytes(signed)));
        await controller.ReceiveSms();
        Assert.True(scope.IsSystem);
        Assert.NotNull(new ResponseService(db).GetResponse(1, recall.recallId));
    }

    [Fact]
    public async Task Resetting_password_revokes_existing_tokens_and_does_not_enable_disabled_accounts()
    {
        await Load(3);
        var controller = new AccessController(db, scope);
        var reset = await controller.ResetPassword(4, new AccessController.PasswordReset("A-new-test-password-123"));
        Assert.IsType<NoContentResult>(reset);
        var user = await db.Users.AsNoTracking().SingleAsync(u => u.Id == 4);
        Assert.True(BCrypt.Net.BCrypt.Verify("A-new-test-password-123", user.Password));
        Assert.Equal(1, user.TokenVersion);
        Assert.False(await access.Load(Principal(4)));
        var renewed = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "4"), new Claim("token_version", "1") }, "test"));
        Assert.True(await access.Load(renewed));
        Assert.Empty(scope.VisibleUnitIds);
        await Load(3);
        Assert.IsType<BadRequestObjectResult>(await controller.UpdateUser(2, new AccessController.UserPermissions(false, true)));
    }

    private async Task<IActionResult> Filter(Type controller, string name, string method, int? id, Dictionary<string, object?> arguments)
    {
        scope.IsSystem = false;
        var descriptor = new ControllerActionDescriptor { ControllerName = controller.Name.Replace("Controller", ""), ActionName = name,
            MethodInfo = controller.GetMethod(name)!, EndpointMetadata = controller.GetMethod(name)!.GetCustomAttributes(true).ToList() };
        var http = new DefaultHttpContext(); http.Request.Method = method;
        if (id.HasValue) http.User = Principal(id.Value);
        var context = new ActionContext(http, new RouteData(), descriptor);
        var executing = new ActionExecutingContext(context, new List<IFilterMetadata>(), arguments, new object());
        await new UnitAuthorizationFilter(access, scope, db).OnActionExecutionAsync(executing,
            () => Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), new object()) { Result = new NoContentResult() }));
        return executing.Result ?? new NoContentResult();
    }
    private sealed class NeverSend : ISmsSender
    {
        public void Send(string recipient, string body) => throw new Exception("Unauthorized SMS reached provider.");
        public Task SendAsync(string recipient, string body) => throw new Exception("Unauthorized SMS reached provider.");
    }
    public void Dispose() { db.Dispose(); connection.Dispose(); }
}
