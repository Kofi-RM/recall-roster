using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using recall_roster.Data;
using recall_roster.Repos;
using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
                .WithOrigins("http://localhost:3000") // Replace with your frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

  //bid.FirstOrDefault()
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
builder.Services.AddScoped<ContactRepository>(); 
builder.Services.AddScoped<RosterRepository>();
builder.Services.AddScoped<RosterContactRepository>();
builder.Services.AddScoped<RecallRepository>();
builder.Services.AddScoped<ResponseRepository>();


// Add your additional services here...


string accountSid = "ACcff994a1cf3f18f1a2ef2140a797c9d6";
        string authToken = "65394a8d245712640d41c5db63dea925";
Console.WriteLine(accountSid);
        TwilioClient.Init(accountSid, authToken);

        /*var message = MessageResource.Create(
            body: "Join Earth's mightiest heroes. Like Kevin Bacon.",
            from: new Twilio.Types.PhoneNumber("+18336223946"),
            to: new Twilio.Types.PhoneNumber("+14707862142")
        ); */

       // Console.WriteLine(message.Sid);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())

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


// Add additional middleware or configure endpoints here...

app.MapControllers();
Console.WriteLine("Fruit salad");
app.Run();

