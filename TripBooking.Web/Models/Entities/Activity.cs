namespace TripBooking.Web.Models.Entities;

public class Activity
{
    public int ActivityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsActive { get; set; }
}
