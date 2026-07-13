using System.ComponentModel.DataAnnotations;

namespace TripBooking.Web.Models.ViewModels;

public class ActivityFormViewModel
{
    // Present but not shown on the Create form (default 0). On Edit, we
    // populate it from the loaded entity and render it as a hidden field
    // (see the Edit view below) so the POST tells us which row to update.
    public int ActivityId { get; set; }

    
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;


    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }


    [Required(ErrorMessage = "Unit price is required.")]
    [Range(0.01, 100000, ErrorMessage = "Unit price must be between 0.01 and 100,000.")]
    public decimal UnitPrice { get; set; }


    public bool IsActive { get; set; } = true;
}
