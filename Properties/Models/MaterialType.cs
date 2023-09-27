using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;
public class MaterialType
{
    public int Id { get; set; }
    [Required] // means that the below property is required to be non-nullable (ie - this value cannot be null)
    public string Name { get; set; }
    public int CheckoutDays { get; set; }
    public List<Material> Materials { get; set; }
}  