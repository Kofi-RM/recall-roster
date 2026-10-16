using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using recall_roster.Data;
using recall_roster.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var demoOnly = builder.Configuration.GetValue<bool>("Demo:Only");
if (demoOnly && !builder.Configuration.GetValue<bool>("Demo:Enabled"))
    throw new InvalidOperationException("Demo:Only requires Demo:Enabled.");
if (builder.Configuration.GetValue<bool>("Demo:Enabled"))
{
    foreach (var key in new[] { "Twilio:AccountSid", "Twilio:AuthToken", "Twilio:FromNumber", "Twilio:VerifyServiceSid", "Demo:StorePath" })
        if (string.IsNullOrWhiteSpace(builder.Configuration[key])) throw new InvalidOperationException($"Configure {key} before enabling the demo.");
}

// Register AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))



);

var frontendOrigins = (builder.Configuration["Frontend:Origins"] ?? "http://localhost:3001,http://localhost:3000,https://micellar-shearless-corazon.ngrok-free.dev").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
var react = builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowOrigin",
          builder => builder
              .WithOrigins(frontendOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
  }); // need to allow access to ngrok url


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Recall Roster", Version = "v1" });
});

if (demoOnly) builder.Services.AddAuthentication();
else builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing configuration is missing.")))
    };
});


// Add services to the container.

builder.Services.AddScoped<UnitAccessScope>();
builder.Services.AddScoped<UnitAccessService>();
builder.Services.AddScoped<UnitAuthorizationFilter>();
builder.Services.AddControllers(options => options.Filters.AddService<UnitAuthorizationFilter>());
builder.Services.AddSingleton<DemoStore>();
builder.Services.AddScoped<IDemoSms, DemoSms>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ISmsSender, TwilioSmsSender>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IRecallResultsService, RecallResultsService>();
builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddScoped<IRosterRepositoryService, RosterRepositoryService>();
builder.Services.AddScoped<IRosterContactService, RosterContactService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Explicit operator-controlled bootstrap. No account becomes administrator merely by registering first.
if (!demoOnly && !string.IsNullOrWhiteSpace(builder.Configuration["Access:BootstrapAdminEmail"]))
{
    using var bootstrap = app.Services.CreateScope();
    var db = bootstrap.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Access.IsSystem = true;
    using var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
    if (!db.Users.Any(u => u.IsSystemAdministrator && u.IsEnabled))
    {
        var email = builder.Configuration["Access:BootstrapAdminEmail"]!.Trim().ToUpperInvariant();
        var user = db.Users.SingleOrDefault(u => u.Email == email && u.IsEnabled)
            ?? throw new InvalidOperationException("Bootstrap requires an existing enabled account with the configured email.");
        user.IsSystemAdministrator = true;
        db.AccessAudits.Add(new recall_roster.Models.AccessAudit { ActorUserId = user.Id, Action = "Bootstrap administrator", Details = "Explicit operator configuration" });
        db.SaveChanges();
    }
    transaction.Commit();
}

// Configure the HTTP request pipeline.

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    context.Response.StatusCode = error is UnauthorizedAccessException ? 403 : 500;
    await context.Response.WriteAsJsonAsync(new { message = "The request could not be completed. Please try again." });
}));
if (demoOnly)
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/api/demo"))
        {
            context.Response.StatusCode = 404;
            return;
        }
        await next();
    });
}
else
{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recall Roster V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

}
app.UseCors("AllowOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
if (demoOnly) app.MapFallbackToFile("index.html");
app.Run();
