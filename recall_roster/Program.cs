using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using recall_roster.Data;
using recall_roster.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Register AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))

git commit -m "Changed Login to Users and added Login/Register DTO. Separated login functionality to AuthController.cs. Added Jwt Service

);

var react = builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowOrigin",
          builder => builder
              .WithOrigins("http://localhost:3001","http://localhost:3000", "https://micellar-shearless-corazon.ngrok-free.dev") // Replace with your frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
  }); // need to allow access to ngrok url


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Recall Roster", Version = "v1" });
});

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = "your-issuer",
        ValidAudience = "your-audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
    };
});


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IRecallResultsService, RecallResultsService>();
builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddScoped<IRosterRepositoryService, RosterRepositoryService>();
builder.Services.AddScoped<IRosterContactService, RosterContactService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recall Roster V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseCors("AllowOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();

