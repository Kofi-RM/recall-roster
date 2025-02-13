﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using recall_roster.Data;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Register AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))


);

var react = builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowOrigin",
          builder => builder
              .WithOrigins("http://localhost:3000", "https://fb38-98-62-120-34.ngrok-free.app") // Replace with your frontend URL
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


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recall Roster V1");
});

app.UseCors("AllowOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();

