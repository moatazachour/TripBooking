using TripBooking.Web.Models.ViewModels;

namespace TripBooking.Web.Services.Interfaces;

public interface IReportService
{
    Task<List<ActivityRevenueReportRow>> GetRevenueByActivityAsync();
    Task<List<DestinationReportRow>> GetTripsByDestinationAsync(int? year);
}