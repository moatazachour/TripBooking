using System.ComponentModel.DataAnnotations;
using TripBooking.Web.Models.ValidationAttributes;

namespace TripBooking.Web.Models.ViewModels;

public class TripFormViewModel
{
    public int TripId { get; set; }

    [Required(ErrorMessage = "Destination is required.")]
    [StringLength(150)]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required.")]
    [FutureOrTodayDate]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    public DateOnly EndDate { get; set; }

    [Required]
    [Range(1, 1000000, ErrorMessage = "Budget must be greater than 0.")]
    public decimal Budget { get; set; }
}