namespace TripBooking.Web.Models.Entities;

public class Traveler
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? PassportNumber { get; set; }
}
