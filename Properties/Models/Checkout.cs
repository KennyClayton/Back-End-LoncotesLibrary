using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;
public class Checkout
{
    public int Id { get; set; }
    [Required]
    public int MaterialId { get; set; }
    public Material Material { get; set; }
    // public int MaterialTypeId { get; set; }
    // public MaterialType MaterialType { get; set; }
    [Required]
    public int PatronId { get; set; }
    public Patron Patron { get; set; } //this navigates through this checkout model to the Patron model
    [Required]
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool Paid { get; set; } // Add a Paid property of type bool to the Checkout class that indicates whether a fee has been paid or not.

    private static decimal _lateFeePerDay = 0.50M; // "The static keyword makes the _lateFeePerDay field belong to the class itself rather than an instance of the class. This means all instances of the Checkout class will share the same _lateFeePerDay value, which is what you likely intend for the late fee per day. Additionally, I changed the value from .50M to 0.50M for clarity; both represent 50 cents but using 0.50M makes it more explicit that it's a decimal value."
 
    public decimal? LateFee
    {
        get
        {
            // To calculate the due date, you need to add the number of days that the item can be checked out to the checkout date:
            DateTime dueDate = CheckoutDate.AddDays(Material.MaterialType.CheckoutDays); 

            // If there is a ReturnDate, and the due date was before the ReturnDate, there will be a fee. Otherwise, the value should be null
            if (ReturnDate != null && dueDate < ReturnDate) 
            {
                //To calculate the fee, you need to find out the number of days between the ReturnDate and the dueDate
                int daysLate = ((DateTime)ReturnDate - dueDate).Days;
                // The actual late fee will be the product of daysLate and the _lateFeePerDay:
                decimal fee = daysLate * _lateFeePerDay;
                return fee;
            }
            return null;
        }
    }


}