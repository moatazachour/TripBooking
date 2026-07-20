using TripBooking.Web.Models.ViewModels;
using TripBooking.Web.Repositories.Interfaces;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<List<ActivityRevenueReportRow>> GetRevenueByActivityAsync()
        => await _reportRepository.GetRevenueByActivityAsync();

    public async Task<List<DestinationReportRow>> GetTripsByDestinationAsync(int? year)
        => await _reportRepository.GetTripsByDestinationAsync(year);
}