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
    // Start with a base query to retrieve all materials.
    var query = db.Materials.AsQueryable(); // AsQueryable "prepares the data source for further filtering, projection, and other operations that you might want to perform using LINQ."
    query = query.Where(material => material.OutOfCirculationSince == null || material.OutOfCirculationSince > DateTime.Now);
    // Apply filters if materialTypeId is provided.
    if (materialTypeId.HasValue)
    {
        query = query.Where(material => material.MaterialTypeId == materialTypeId.Value);
    }

    // Apply filters if genreId is provided.
    if (genreId.HasValue)
    {
        query = query.Where(material => material.GenreId == genreId.Value);
    }

    // Execute the final query and return the results.
    var materials = await query.ToListAsync();

    // Return the materials as JSON.
    return Results.Ok(materials);
});


//^ ENDPOINT - Get all materials (with their genre and material type) that are currently available (not checked out, and not removed from circulation). A checked out material will have a related checkout that has a ReturnDate of null.
app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    // .Include(m => m.Genre)
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null)) // "The second Where says "only return materials where all of the material's checkouts have a value for ReturnDate"
    .ToList();
});


//^ ENDPOINT - Get Material details
// The librarians would like to see details for a material. Include the Genre, MaterialType, and Checkouts (as well as the Patron associated with each checkout using ThenInclude).
app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    var material = db.Materials
    .Where(m => m.Id == id)
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Include(m => m.Checkouts)
        .ThenInclude(c => c.Patron)
    .FirstOrDefault();
    if (material == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(material);
});


//^ ENDPOINT - Add a Material
// Materials are often added to the library's collection. Add an endpoint to create a new material -- get this by material Id
app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) => //a lambda expression that defines an anonymous function with two parameters. It receives an instance of LoncotesLibraryDbContext named db, which represents a database context used for interacting with the database. It also receives an object of type Material named material, which represents the material data that will be sent in the request body.
{
    db.Materials.Add(material); //add the "material" object that we give it in Postman to the Materials list
    db.SaveChanges(); //"This line effectively inserts the new material data into the database. It's essential to call SaveChanges after adding or modifying data in the context to ensure the changes are saved to the underlying database."
    return Results.Created($"/api/materials/{material.Id}", material); //"This indicates that the material has been successfully created and provides a URL to the newly created resource. It uses the Results.Created method to create an HTTP 201 Created response, which is a common status code for indicating successful resource creation. The URL to the newly created material is formed using the material.Id."
});


//^ ENDPOINT - Remove a Material From Circulation
//Add an endpoint that expects an id in the url, which sets the OutOfCirculationSince property of the material that matches the material id to DateTime.Now. (This is called a soft delete, where a row is not deleted from the database, but instead has a flag that says the row is no longer active.) The endpoint to get all materials should already be filtering these items out.
app.MapPut("/api/materials/{id}", (LoncotesLibraryDbContext db, int id, Material material) =>
{
    // Retrieve the material by ID from the database.
    Material materialToUpdate = db.Materials.SingleOrDefault(material => material.Id == id);

    // Check if the material exists.
    if (materialToUpdate == null)
    {
        return Results.NotFound();
    }

    // Set the OutOfCirculationSince property to the current date and time (soft delete).
    materialToUpdate.OutOfCirculationSince = DateTime.Now;

    // Save the changes to the database.
    db.SaveChanges();

    // Return a success response.
    return Results.Ok("Material removed from circulation.");
});


//^ ENDPOINT - Get MaterialTypes
// The librarians will need a form in their app that lets them choose material types. Create an endpoint that retrieves all of the material types to eventually populate that form field
app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes.ToList();
});


//^ ENDPOINT - Get Genres
// The librarians will also need form fields that have all of the genres to choose from. Create an endpoint that gets all of the genres.
app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.ToList();
});


//^ ENDPOINT - Get all Patrons
// The librarians will also need form fields that have all of the genres to choose from. Create an endpoint that gets all of the genres.
app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{
    return db.Patrons.ToList();
});


//^ ENDPOINT - Get Patron with Checkouts (using Include and ThenInclude and SingleOrDefault)
// This endpoint should get a patron and include their checkouts, and further include the materials and their material types.
app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) => //*  LoncotesLibraryDbContext is used to access the database, and id is the ID of the patron we want to retrieve.
{
    // Retrieve the patron by ID from the database, including their checkouts and related material information.
    //Notice this actually builds a list of all patrons FIRST and then we single out the one we want with SingleOrDefault
    return db.Patrons // patron is a single instance of the patrons list...which one though? we define that lower down
        .Include(p => p.Checkouts) // include the list of checkouts of the patron AND THEN INCLUDE, from that Checkout object, all of its materials and THEN include all of the materialType data within that Material...
        .ThenInclude(c => c.Material) // again, here we include the Checkout's material objects related to that checkout
        .ThenInclude(mt => mt.MaterialType) // and here we include the Checkout's material type 
        .SingleOrDefault(p => p.Id == id); // now single out the one primary key/Id that matches the one we specify in the url
});


//^ ENDPOINT - Update Patron
// Sometimes patrons move or change their email address. Add an endpoint that updates these properties only.
app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id, Patron patron) =>  // add a Patron instance since we need to update a Patron object
{
    //grab an instance of patron so we can update it
    //reference the properties we need to update (email and address)
    //return the updated instance of that patron
    Patron patronToUpdate = db.Patrons.SingleOrDefault(patron => patron.Id == id); // this new patron object is the one in the database, Patrons list, matching Id
    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }
    patronToUpdate.Email = patron.Email;
    patronToUpdate.Address = patron.Address;
    patronToUpdate.IsActive = patron.IsActive;
    db.SaveChanges();
    return Results.NoContent();
}
);


//^ ENDPOINT - Deactivate Patron
// Sometimes patrons move out of the county. Allow the librarians to deactivate a patron (another soft delete example!).
app.MapPut("/api/patrons/deactivate/{id}", (LoncotesLibraryDbContext db, int id, Patron patron) =>
{
    Patron patronToUpdate = db.Patrons.SingleOrDefault(patron => patron.Id == id);
    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }
    patronToUpdate.IsActive = false;
    db.SaveChanges();
    return Results.NoContent();
});


//^ ENDPOINT - Checkout a Material
// The librarians need to be able to checkout items for patrons. Add an endpoint to create a new Checkout for a material and patron. Automatically set the checkout date to DateTime.Today.
app.MapPost("/api/patrons/{patronId}/checkouts", (LoncotesLibraryDbContext db, int patronId, Checkout newCheckout) =>
{
    //This variable holds a single patron instance of the Librarian's choosing (example, patron 4)
    Patron patronToUpdate = db.Patrons.FirstOrDefault(p => p.Id == patronId);
    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }
    // now hold the materialTypeId selected by the Librarian in a variable. 
    Material materialToUpdate = db.Materials.FirstOrDefault(m => m.Id == newCheckout.MaterialId);
    if (materialToUpdate == null)
    {
        return Results.NotFound();
    }
    //set the new instance with its new property values
    newCheckout.Patron = patronToUpdate;
    newCheckout.Material = materialToUpdate;
    newCheckout.CheckoutDate = DateTime.Today;

    db.Checkouts.Add(newCheckout);
    db.SaveChanges();

    return Results.Created($"/api/checkouts/{newCheckout.Id}", newCheckout);
});


//^ ENDPOINT - Get all checkouts
app.MapGet("/api/checkouts", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts.ToList();
});


//^ ENDPOINT - Return a Material
// The librarians need an endpoint to mark a checked out item as returned by item id. Add an endpoint expecting a checkout id, and update the checkout with a return date of DateTime.Today.
app.MapPut("/api/checkouts/return/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    // Find the checkout record by ID.
    var checkout = db.Checkouts.Find(id);
    if (checkout == null)
    {
        return Results.NotFound("Checkout not found.");
    }
    // Check if the item is already returned (has a non-null return date).
    if (checkout.ReturnDate != null)
    {
        return Results.BadRequest("Item is already returned.");
    }
    // Update the checkout with the return date set to today.
    checkout.ReturnDate = DateTime.Today;

    // Save changes to the database.
    db.SaveChanges();

    return Results.NoContent();
});



//^ ENDPOINT - Get overdue checkouts
app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db) => // "This part of the code is an inline lambda function that takes an instance of LoncotesLibraryDbContext as a parameter. It uses this database context to query the database."
{
    return db.Checkouts //query the checkouts table using the db context we passed as a parameter
    .Include(p => p.Patron) //this gives us a list of all patrons with checkouts...//*"loads the Patron navigation property of each Checkout so that you have access to patron information associated with each checkout."
    .Include(co => co.Material)
    .ThenInclude(m => m.MaterialType) //"The ThenInclude method is used to specify a nested related property. It includes the MaterialType navigation property of each Material, which is nested under the Material navigation property."
    .Where(co => // use where to filter all above results down to only those checkouts WHERE the checkout is beyond its due date (which is based on the checkoutdays property compared to how many days it has been out)
    (DateTime.Today - co.CheckoutDate).Days >
        co.Material.MaterialType.CheckoutDays &&
        co.ReturnDate == null)
        .ToList();
});

//overdue checkouts would include: patron, material, checkoutdate, returndate


app.Run();

