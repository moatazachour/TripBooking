using System.ComponentModel.DataAnnotations;

namespace TripBooking.Web.Models.ValidationAttributes;

// Inheriting from ValidationAttribute plugs directly into the same
// automatic validation pipeline described in 5.1 — this runs at the exact
// same point [Required] and [Range] do, no special wiring needed anywhere.
public class FutureOrTodayDateAttribute : ValidationAttribute
{
    // IsValid is the one method you must override. It's called automatically
    // by the validation system during model binding, with "value" being
    // whatever the bound property's current value is.
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly date)
        {
            if (date < DateOnly.FromDateTime(DateTime.Today))
            {
                // Returning a ValidationResult (rather than null) is how you
                // signal failure. The string becomes the error message shown
                // in <span asp-validation-for="...">, identical to a built-in
                // attribute's ErrorMessage.
                return new ValidationResult("Date cannot be in the past.");
            }
        }

        // Returning ValidationResult.Success (or null) means validation passed.
        return ValidationResult.Success;
    }
}