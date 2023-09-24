using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;
public class MaterialType
{
    public int Id { get; set; }
    [Required] // means that the below property is required to be non-nullable (ie - this value cannot be null)
    public string Name { get; set; }
    public int CheckoutDays { get; set; } //? if this is a calculated property I need to make sure I give this model access to the calculating properties using Composition
}