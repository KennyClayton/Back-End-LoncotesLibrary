using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;
public class Patron
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public List<Checkout> Checkouts { get; set; }

    public decimal Balance
    {
        get
        {
            // Calculate the total unpaid fines by summing up the late fees for each checkout.
            decimal totalUnpaidFines = Checkouts //start with list of checkouts and then...
                .Where(co => co.LateFee.HasValue) // ...filter checkouts with late fees...and then...
                .Sum(co => co.LateFee.Value); // ...sum the late fees.

            return totalUnpaidFines;
        }

    }
} 