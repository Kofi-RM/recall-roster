using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using recall_roster.Data;
using recall_roster.Models;
using recall_roster.DTOs;
namespace recall_roster.Services;

public sealed class UnitAuthorizationFilter(UnitAccessService access, UnitAccessScope scope, AppDbContext db) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any()) { await next(); return; }
        if (context.HttpContext.User.Identity?.IsAuthenticated != true || !await access.Load(context.HttpContext.User))
        { context.Result = new UnauthorizedResult(); return; }
        var action = (ControllerActionDescriptor)context.ActionDescriptor;
        var controller = action.ControllerName;
        var method = context.HttpContext.Request.Method;
        if (controller is "Access" or "Auth" or "User")
        {
            if (controller == "Access" && action.ActionName == "Me") { await next(); return; }
            if (!scope.IsAdministrator) { context.Result = new ForbidResult(); return; }
            await next(); return;
        }
        int? unitId = null;
        int ReadId(string key) => context.ActionArguments.TryGetValue(key, out var value) && value is int number ? number : 0;
        var id = ReadId("id");
        var recallId = ReadId("recallId");
        switch (controller)
        {
            case "Recall":
                if (method == "POST")
                {
                    var input = context.ActionArguments.Values.OfType<CreateRecallRequest>().Single();
                    unitId = await db.Rosters.Where(r => r.rosterId == input.rosterId).Select(r => (int?)r.UnitId).SingleOrDefaultAsync();
                }
                else if (id > 0) unitId = await db.Recalls.Where(r => r.recallId == id).Select(r => (int?)r.UnitId).SingleOrDefaultAsync();
                break;
            case "Message":
                unitId = await db.Recalls.Where(r => r.recallId == recallId).Select(r => (int?)r.UnitId).SingleOrDefaultAsync();
                break;
            case "Roster":
                unitId = method == "POST" ? context.ActionArguments.Values.OfType<Roster>().Single().UnitId
                    : id > 0 ? await db.Rosters.Where(r => r.rosterId == id).Select(r => (int?)r.UnitId).SingleOrDefaultAsync() : null;
                break;
            case "Contact":
                unitId = method == "POST" ? context.ActionArguments.Values.OfType<ContactCreateDto>().Single().UnitId
                    : id > 0 ? await db.Contacts.Where(c => c.contactId == id).Select(c => (int?)c.UnitId).SingleOrDefaultAsync() : null;
                break;
            case "RosterContact":
                var rosterId = ReadId("rosterId");
                if (rosterId == 0) rosterId = id;
                if (rosterId == 0) rosterId = context.ActionArguments.Values.OfType<UpdateRosterContacts>().FirstOrDefault()?.RosterId
                    ?? context.ActionArguments.Values.OfType<RosterContact>().FirstOrDefault()?.rosterId ?? 0;
                unitId = await db.Rosters.Where(r => r.rosterId == rosterId).Select(r => (int?)r.UnitId).SingleOrDefaultAsync();
                break;
            case "Response":
                if (recallId > 0) unitId = await db.Recalls.Where(r => r.recallId == recallId).Select(r => (int?)r.UnitId).SingleOrDefaultAsync();
                break;
            default: context.Result = new ForbidResult(); return;
        }
        if (method != "GET")
        {
            if (unitId == null) { context.Result = new NotFoundResult(); return; }
            var allowed = controller is "Recall" or "Message" ? scope.CanInitiate(unitId.Value) : scope.CanManage(unitId.Value);
            if (!allowed) { context.Result = new ForbidResult(); return; }
        }
        await next();
    }
}
