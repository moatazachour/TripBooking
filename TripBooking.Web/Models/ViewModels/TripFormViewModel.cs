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

    // Also carried silently through Edit as a hidden field (added here in
    // Chapter 8, once file uploads existed). Without this property, Edit's
    // Trip_Update call has no way to know the trip already had a document —
    // it would send DocumentPath = null every time, WIPING OUT a previously
    // uploaded file's path on every single edit, even if you only changed
    // the destination. This is the exact class of bug Chapter 4.2 warned
    // about: a form/ViewModel that doesn't carry a field it needs to
    // preserve, silently corrupting data that isn't even part of the form.
    public string? DocumentPath { get; set; }
}