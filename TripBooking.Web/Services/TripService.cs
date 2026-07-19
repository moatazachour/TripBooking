using TripBooking.Web.Models.Entities;
using TripBooking.Web.Repositories.Interfaces;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;

    public TripService(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<List<Trip>> GetAllTripsAsync() => await _tripRepository.GetAllAsync();

    public async Task<List<Trip>> GetTripsForTravelerAsync(int travelerId)
        => await _tripRepository.GetByTravelerIdAsync(travelerId);

    public async Task<Trip?> GetTripByIdAsync(int tripId) => await _tripRepository.GetByIdAsync(tripId);

    public async Task<Trip> CreateTripAsync(Trip trip) => await _tripRepository.InsertAsync(trip);

    public async Task UpdateTripAsync(Trip trip) => await _tripRepository.UpdateAsync(trip);

    public async Task DeleteTripAsync(int tripId) => await _tripRepository.DeleteAsync(tripId);
    
    private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "Destination", "Budget", "StartDate", "CreatedAtUtc"
    };

    public async Task<(List<Trip> Trips, int TotalCount)> SearchTripsAsync(
        string? destinationFilter, string? statusFilter,
        string sortColumn, string sortDirection,
        int pageNumber, int pageSize)
    {
        // Why validate sortColumn here, in the Service, instead of trusting
        // whatever came in from the query string? Because sortColumn ultimately
        // ends up as part of the logic executed inside Trip_Search's CASE
        // statements. Our stored procedure only recognizes 4 known column
        // names, so a bad value here just silently falls through to the
        // default ordering — not dangerous by itself with THIS particular
        // procedure's design, but it's a good habit to validate any value
        // that influences SQL execution before it leaves C#, rather than
        // relying on the stored procedure being defensively written every time.
        var safeSortColumn = AllowedSortColumns.Contains(sortColumn) ? sortColumn : "CreatedAtUtc";
        var safeSortDirection = sortDirection.Equals("ASC", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

        // Clamp paging inputs to sane bounds. A user (or someone poking at the
        // URL directly) could otherwise request PageSize=999999 and force the
        // database to hand back the entire table anyway — defeating the whole
        // point of paging. Never trust paging parameters straight from a query
        // string without a sanity check.
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Clamp(pageSize, 1, 100);

        return await _tripRepository.SearchAsync(
            destinationFilter, statusFilter, safeSortColumn, safeSortDirection, safePageNumber, safePageSize);
    }
}