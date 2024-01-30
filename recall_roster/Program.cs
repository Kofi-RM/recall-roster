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

var builder = WebApplication.CreateBuilder(args);

// Register AppDbContext
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            

// );
DotNetEnv.Env.Load();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Debug);

});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ContactRepository>(); 

// Add your additional services here...


string accountSid = "ACcff994a1cf3f18f1a2ef2140a797c9d6";
        string authToken = "65394a8d245712640d41c5db63dea925";
Console.WriteLine(accountSid);
        TwilioClient.Init(accountSid, authToken);

        var message = MessageResource.Create(
            body: "Join Earth's mightiest heroes. Like Kevin Bacon.",
            from: new Twilio.Types.PhoneNumber("+18336223946"),
            to: new Twilio.Types.PhoneNumber("+14707862142")
        );

        Console.WriteLine(message.Sid);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())

    app.UseDeveloperExceptionPage();
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name V1");
});



// Add additional middleware or configure endpoints here...

app.MapControllers();
Console.WriteLine("Fruit salad");
app.Run();

