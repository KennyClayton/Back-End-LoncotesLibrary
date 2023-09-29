using Microsoft.EntityFrameworkCore; //namespaces allow us to "use classes and types from those namespaces without having to specify their fully qualified names"
using LoncotesLibrary.Models; // "This includes types from the LoncotesLibrary.Models namespace, which contains the entity classes that correspond to database tables."

//* Inheritance - "Inheritance means that a class inherits all of the properties, fields, and methods of another class. All of the properties of DbContext allow the LoncotesLibraryDbContext class to connect to the database with no other code that you have to write."
public class LoncotesLibraryDbContext : DbContext // this is a subclass because it is inheriting from the DBContext class which is a class we got from Entity Framework Core. It manages database interactions. From the curriculum "DbContext is a class that comes from EF Core that represents our database as .NET objects that we can access"
{
    public DbSet<Genre> Genres { get; set; } //"DbSet Properties: Inside the LoncotesLibraryDbContext class, there are several DbSet properties. Each DbSet represents an entity (or a table) in the database. These properties allow you to interact with these entities in your C# code as if they were regular C# objects."
    public DbSet<Material> Materials { get; set; } // this property represents a DbSet for the "Materials" entity, since there is a corresponding table in the database called "Materials."
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Patron> Patrons { get; set; }
    public DbSet<Checkout> Checkouts { get; set; }

    //This explains the below base class and is copied from the CreekRiver project: "Finally, there is something that looks like a method called CreekRiverDbContext. This is a constructor, which is a method-like member of a class that allows us to write extra logic to configure the class, so that it is ready for use when it is created. You can always tell that something is a constructor in a class when: 1. It is public, 2. has the same name as the class itself, and 3. has no return type. In this case, our CreekRiverDbContext class actually doesn't need any special setup, but the DbContext class does. DbContext is our class's base class, and it requires an options object to set itself up properly. We use the base keyword to pass that object down to DbContext when ASP.NET creates the CreekRiverDbContext class."
    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //* OnModeling - is used to create database
    //* Protected - "protected, in contrast, means that the method can only be called from code inside the class itself, or by any class that inherits it. This is a form of encapsulation, which means keeping code that is only safe or useful to use inside a particular context inaccessible to other parts of the program."
    //* Override - "indicates that OnModelCreating is actually replacing a method of the same name that is inherited from the DbContext class. Such methods, like the one in the DbContext class, are marked with the virtual keyword."
    //* "The rest of the code in this method will check - every time we create or update the database schema - whether this data is in the database or not, and will attempt to add it if it doesn't find it all. This is very useful for seeding the database when it is created for the first time with test data."
    {
        //^ CHECKOUT
            //* HasData - this method is from Entity Framework Core and it allows us to seed data for a database when we are initalizing or migrating a database
        modelBuilder.Entity<Checkout>().HasData(new Checkout[]
        {
            new Checkout { Id = 1, MaterialId = 1, PatronId = 1, CheckoutDate = new DateTime(2023, 07, 25), ReturnDate = null }, // 30 days before late return
            new Checkout { Id = 2, MaterialId = 2, PatronId = 2, CheckoutDate = new DateTime(2023, 08, 20), ReturnDate = null }, // 40 days before late return
            new Checkout { Id = 3, MaterialId = 3, PatronId = 3, CheckoutDate = new DateTime(2023, 09, 18), ReturnDate = null }, // 30 days before late return
            new Checkout { Id = 4, MaterialId = 4, PatronId = 4, CheckoutDate = new DateTime(2023, 09, 16), ReturnDate = null }, // this one was never checked out
            // new Checkout { Id = 5, MaterialId = 5, PatronId = 1, CheckoutDate = new DateTime(2023, 09, 16), ReturnDate = DateTime.Now },
            // new Checkout { Id = 6, MaterialId = 6, PatronId = 2, CheckoutDate = new DateTime(2023, 09, 15), ReturnDate = DateTime.Now },
            // new Checkout { Id = 7, MaterialId = 7, PatronId = 3, CheckoutDate = new DateTime(2023, 09, 10), ReturnDate = DateTime.Now },
            // new Checkout { Id = 8, MaterialId = 8, PatronId = 2, CheckoutDate = new DateTime(2023, 09, 08), ReturnDate = DateTime.Now },
            // new Checkout { Id = 9, MaterialId = 9, PatronId = 1, CheckoutDate = new DateTime(2023, 09, 02), ReturnDate = new DateTime (2023, 09, 03) },
            // new Checkout { Id = 10, MaterialId = 10, PatronId = 4, CheckoutDate = new DateTime(2023, 09, 01), ReturnDate = DateTime.Now },
        });

        //^ GENRE
        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new Genre { Id = 1, Name = "Sports" },
            new Genre { Id = 2, Name = "History" },
            new Genre { Id = 3, Name = "Mathematics" },
            new Genre { Id = 4, Name = "Sci-Fi" },
            new Genre { Id = 5, Name = "Food" }
        }); 

        //^ MATERIAL
        modelBuilder.Entity<Material>().HasData(new Material[]
        {
            new Material { Id = 1, MaterialName = "Book on Sports", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null },
            new Material { Id = 2, MaterialName = "History Magazine", MaterialTypeId = 2, GenreId = 2, OutOfCirculationSince = DateTime.Now.AddDays(-30) },
            new Material { Id = 3, MaterialName = "Mathematics Textbook", MaterialTypeId = 1, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 4, MaterialName = "Sci-Fi CD", MaterialTypeId = 3, GenreId = 4, OutOfCirculationSince = DateTime.Now.AddDays(-15) },
            new Material { Id = 5, MaterialName = "Food Recipe Book", MaterialTypeId = 1, GenreId = 5, OutOfCirculationSince = null },
            new Material { Id = 6, MaterialName = "Sports Biography", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = DateTime.Now.AddDays(-60) },
            new Material { Id = 7, MaterialName = "Mathematics Journal", MaterialTypeId = 2, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 8, MaterialName = "Sci-Fi Novel", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = DateTime.Now.AddDays(-45) },
            new Material { Id = 9, MaterialName = "Cooking Magazine", MaterialTypeId = 2, GenreId = 5, OutOfCirculationSince = null },
            new Material { Id = 10, MaterialName = "History Textbook", MaterialTypeId = 1, GenreId = 2, OutOfCirculationSince = DateTime.Now.AddDays(-75) }
        });

        //^ MATERIAL TYPE
        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
              {
            new MaterialType { Id = 1, Name = "Book", CheckoutDays = 30},
            new MaterialType { Id = 2, Name = "Periodical", CheckoutDays = 40},
            new MaterialType { Id = 3, Name = "CD", CheckoutDays = 60}
              });

        //^ PATRON
        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
            new Patron { Id = 1, FirstName = "Clark", LastName = "Howard", Address = "123 Main St", Email = "clark@howard.com", IsActive = false},
            new Patron { Id = 2, FirstName = "Howie", LastName = "Clarkerston", Address = "321 Branch St", Email = "howie@clarkerston.com", IsActive = true},
            new Patron { Id = 3, FirstName = "Mark", LastName = "Markly", Address = "100 Airpot Blvd", Email = "mark@markly.com", IsActive = true},
            new Patron { Id = 4, FirstName = "Deion", LastName = "Sanderson", Address = "5th Street", Email = "deion@sanderson.com", IsActive = true}
        });
    }

}


// "Data Access: Once you've defined your DbContext and DbSet properties, you can use instances of LoncotesLibraryDbContext to perform various database operations, such as querying, inserting, updating, and deleting records from the associated tables. For example, you can create an instance of LoncotesLibraryDbContext and use it to retrieve data from the "Genres" table or insert new records into the "Materials" table."

//* LEARN - "In summary, the code you provided sets up a custom database context class using Entity Framework Core, defines DbSet properties to represent database tables, and enables interaction with the database using C# classes and LINQ queries. This is a fundamental structure for building database-driven applications in C# with Entity Framework Core."