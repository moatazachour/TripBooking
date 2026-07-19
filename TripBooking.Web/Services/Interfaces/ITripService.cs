using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Services.Interfaces;

public interface ITripService
{
    Task<List<Trip>> GetAllTripsAsync();
    Task<List<Trip>> GetTripsForTravelerAsync(int travelerId);
    Task<Trip?> GetTripByIdAsync(int tripId);
    Task<int> CreateTripAsync(Trip trip);
    Task UpdateTripAsync(Trip trip);
    Task DeleteTripAsync(int tripId);
    Task<(List<Trip> Trips, int TotalCount)> SearchTripsAsync(
    string? destinationFilter, string? statusFilter,
    string sortColumn, string sortDirection,
    int pageNumber, int pageSize);
}