using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;
public class Checkout
{
    public int Id { get; set; }
    [Required]
    public int MaterialId { get; set; }
    public Material Material { get; set; }
    public int MaterialTypeId { get; set; }
    public MaterialType MaterialType { get; set; }
    [Required]
    public string PatronId { get; set; }
    [Required]
    public DateTime? CheckoutDate { get; set; }
    [Required]
    public DateTime ReturnDate { get; set; }  
}