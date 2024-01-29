using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using recall_roster.Data;
using recall_roster.Repos;


using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Register AppDbContext
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            

// );
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

