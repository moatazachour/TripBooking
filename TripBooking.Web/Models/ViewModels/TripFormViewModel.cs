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

    // Carried silently through Edit's round trip as a hidden field — this is
    // what powers the optimistic concurrency check from 6.6. Never rendered
    // as a visible/editable field; the user never sees or touches it directly.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}