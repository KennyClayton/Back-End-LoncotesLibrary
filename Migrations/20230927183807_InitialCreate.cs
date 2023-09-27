using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CheckoutDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialName = table.Column<string>(type: "text", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false),
                    OutOfCirculationSince = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materials_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    PatronId = table.Column<int>(type: "integer", nullable: false),
                    CheckoutDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkouts_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Checkouts_Patrons_PatronId",
                        column: x => x.PatronId,
                        principalTable: "Patrons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Sports" },
                    { 2, "History" },
                    { 3, "Mathematics" },
                    { 4, "Sci-Fi" },
                    { 5, "Food" }
                });

            migrationBuilder.InsertData(
                table: "MaterialTypes",
                columns: new[] { "Id", "CheckoutDays", "Name" },
                values: new object[,]
                {
                    { 1, 30, "Book" },
                    { 2, 40, "Periodical" },
                    { 3, 60, "CD" }
                });

            migrationBuilder.InsertData(
                table: "Patrons",
                columns: new[] { "Id", "Address", "Email", "FirstName", "IsActive", "LastName" },
                values: new object[,]
                {
                    { 1, "123 Main St", "clark@howard.com", "Clark", false, "Howard" },
                    { 2, "321 Branch St", "howie@clarkerston.com", "Howie", true, "Clarkerston" },
                    { 3, "100 Airpot Blvd", "mark@markly.com", "Mark", true, "Markly" },
                    { 4, "5th Street", "deion@sanderson.com", "Deion", true, "Sanderson" }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "GenreId", "MaterialName", "MaterialTypeId", "OutOfCirculationSince" },
                values: new object[,]
                {
                    { 1, 1, "Book on Sports", 1, null },
                    { 2, 2, "History Magazine", 2, new DateTime(2023, 8, 28, 14, 38, 7, 685, DateTimeKind.Local).AddTicks(5791) },
                    { 3, 3, "Mathematics Textbook", 1, null },
                    { 4, 4, "Sci-Fi CD", 3, new DateTime(2023, 9, 12, 14, 38, 7, 685, DateTimeKind.Local).AddTicks(5823) },
                    { 5, 5, "Food Recipe Book", 1, null },
                    { 6, 1, "Sports Biography", 1, new DateTime(2023, 7, 29, 14, 38, 7, 685, DateTimeKind.Local).AddTicks(5826) },
                    { 7, 3, "Mathematics Journal", 2, null },
                    { 8, 4, "Sci-Fi Novel", 1, new DateTime(2023, 8, 13, 14, 38, 7, 685, DateTimeKind.Local).AddTicks(5828) },
                    { 9, 5, "Cooking Magazine", 2, null },
                    { 10, 2, "History Textbook", 1, new DateTime(2023, 7, 14, 14, 38, 7, 685, DateTimeKind.Local).AddTicks(5830) }
                });

            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "Id", "CheckoutDate", "MaterialId", "PatronId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, null },
                    { 2, new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, null },
                    { 3, new DateTime(2023, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, null },
                    { 4, new DateTime(2023, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 4, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MaterialId",
                table: "Checkouts",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_PatronId",
                table: "Checkouts",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_GenreId",
                table: "Materials",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Checkouts");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MaterialTypes");
        }
    }
}
