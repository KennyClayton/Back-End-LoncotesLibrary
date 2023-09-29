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

    public decimal? Balance // to get a patron's balance we need to know if the patron has any overdue books. To know if it has any overdue books we have to compare the checkoutDays to the checkout today compared to today. Then we find tht difference and if it's over the checkoutDays limit then they should be charged a fee.
    {
        get
        {
            if (Checkouts != null) // first, if the patron has any checkouts at all, do this...
            {
                decimal totalLateFees = 0; //initialize totalLateFees at zero...

                foreach (var checkout in Checkouts) //...and for each checkout instance in the checkouts list for this Patron //#REMEMBER! we are sitting in the Patron class right now, so this code is checking against a single Patron instance right now...
                {
                    if (!checkout.Paid && checkout.LateFee.HasValue) //...if both conditions are true, that the checkout paid property is false AND the late fee property has a value at all...then...
                    {
                        totalLateFees += checkout.LateFee.Value; //... add the totalLateFees above to the value of the LateFee property
                    }
                }
                return totalLateFees;
            }
            return null; //otherwise, return null if there is no checkout for this Patron
        }
    }
} 