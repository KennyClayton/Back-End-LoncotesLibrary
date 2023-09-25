// //* BELOW - This is how we configured Program.cs to use EF Core and Npgsql
using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

//* allows our api endpoints to access the database through Entity Framework Core
//* BELOW - This is additionally how we configured Program.cs to use EF Core and Npgsql
// also below, "builder.Configuration["LoncotesLibraryDbConnectionString"] retrieves the connection string that we stored in the secrets manager so that EF Core can use it to connect to the database. Don't worry about what the others are doing for now."
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);


// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
//* ABOVE - This is how we configured Program.cs to use EF Core and Npgsql

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



















app.Run();

