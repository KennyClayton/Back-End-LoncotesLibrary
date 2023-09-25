using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Material
{
    public int Id { get; set; }
    [Required] // means that the below property is required to be non-nullable (ie - this value cannot be null)
    public string MaterialName { get; set; }
    [Required]
    public int MaterialTypeId { get; set; }
    [Required]
    public MaterialType MaterialType { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
    public Checkout Checkout { get; set; }
    public DateTime? OutOfCirculationSince { get; set; } //think...why can this be nullable? Because this may or may not have a date in this field...The material may still be in circulation, and if so then it needs not date here
} 