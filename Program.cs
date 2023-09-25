// //* BELOW - This is how we configured Program.cs to use EF Core and Npgsql
using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using System.Runtime.InteropServices;

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



//^ ENDPOINT - Get all Materials (combined this with the one below per instructions/curriculum)
// The librarians would like to see a list of all the circulating materials. Include the Genre and MaterialType. Exclude materials that have a OutOfCirculationSince value.
// Notes: this is a parameterized lambda expression that takes a LoncotesLibraryDbContext parameter named db AND asks if the HTTP request also included an additional piece of query for materialType or genre with the question mark
// app.MapGet("/api/materials", (LoncotesLibraryDbContext db) => 
// {
//     return db.Materials.ToList(); //This code is using the provided db parameter (which is an instance of LoncotesLibraryDbContext) to interact with the database. It says "Go to the database, look at Materials table, get the list of instances."
// });


//^ ENDPOINT - Get Materials by Genre and/or MaterialType
// The librarians also like to search for materials by genre and type. Add query string parameters to the above endpoint for materialTypeId and genreId. Update the logic of the above endpoint to include both, either, or neither of these filters, depending which are passed in. Remember, query string parameters are always optional when making an HTTP request, so you have to account for the possibility that any of them will be missing.
app.MapGet("/api/materials", async (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
{
    // If neither materialTypeId nor genreId is specified, return all materials.
    if (materialTypeId == null && genreId == null)
    {
        return await db.Materials.ToListAsync();
    }

    // If materialTypeId is specified, filter by material type.
    if (materialTypeId != null)
    {
        return await db.Materials.Where(m => m.MaterialTypeId == materialTypeId).ToListAsync();
    }

    // If genreId is specified, filter by genre.
    if (genreId != null)
    {
        return await db.Materials.Where(m => m.GenreId == genreId).ToListAsync();
    }

    // If both materialTypeId and genreId are specified, filter by both.
    return await db.Materials.Where(m => m.MaterialTypeId == materialTypeId && m.GenreId == genreId).ToListAsync();
});





//^ ENDPOINT - Get Material details
// The librarians would like to see details for a material. Include the Genre, MaterialType, and Checkouts (as well as the Patron associated with each checkout using ThenInclude).
app.MapGet("/api/materials", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Include(m => m.Checkout)
    .ThenInclude(c => c.Patron)
    .ToList();
});



//^ ENDPOINT - Add a Material (by Id)
// Materials are often added to the library's collection. Add an endpoint to create a new material -- get this by material Id





//^ ENDPOINT - Remove a Material From Circulation
//Add an endpoint that expects an id in the url, which sets the OutOfCirculationSince property of the material that matches the material id to DateTime.Now. (This is called a soft delete, where a row is not deleted from the database, but instead has a flag that says the row is no longer active.) The endpoint to get all materials should already be filtering these items out.




//^ ENDPOINT - Get MaterialTypes
// The librarians will need a form in their app that let's them choose material types. Create an endpoint that retrieves all of the material types to eventually populate that form field



//^ ENDPOINT - Get Genres
// The librarians will also need form fields that have all of the genres to choose from. Create an endpoint that gets all of the genres.




//^ ENDPOINT - Get Patron with Checkouts
// This endpoint should get a patron and include their checkouts, and further include the materials and their material types.




//^ ENDPOINT - Update Patron
// Sometimes patrons move or change their email address. Add an endpoint that updates these properties only.




//^ ENDPOINT - Deactivate Patron
// Sometimes patrons move out of the county. Allow the librarians to deactivate a patron (another soft delete example!).




//^ ENDPOINT - Checkout a Material
// The librarians need to be able to checkout items for patrons. Add an endpoint to create a new Checkout for a material and patron. Automatically set the checkout date to DateTime.Today.




//^ ENDPOINT - Return a Material
// The librarians need an endpoint to mark a checked out item as returned by item id. Add an endpoint expecting a checkout id, and update the checkout with a return date of DateTime.Today.






app.Run();

