namespace TripBooking.Web.Models.ViewModels;

public class ActivityRevenueReportRow
{
    public int ActivityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalUnitsBooked { get; set; }
    public int TripCount { get; set; }
}