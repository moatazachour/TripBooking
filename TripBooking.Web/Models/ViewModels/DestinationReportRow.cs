namespace TripBooking.Web.Models.ViewModels;

public class DestinationReportRow
{
    public string Destination { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AverageBudget { get; set; }
}