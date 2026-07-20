using TripBooking.Web.Models.ViewModels;

namespace TripBooking.Web.Repositories.Interfaces;

public interface IReportRepository
{
    Task<List<ActivityRevenueReportRow>> GetRevenueByActivityAsync();
    Task<List<DestinationReportRow>> GetTripsByDestinationAsync(int? year);
}