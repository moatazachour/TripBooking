using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Repositories.Interfaces;

public interface ITripRepository
{
    Task<List<Trip>> GetAllAsync();
    Task<List<Trip>> GetByTravelerIdAsync(int travelerId);
    Task<Trip?> GetByIdAsync(int tripId);
    Task<int> InsertAsync(Trip trip);
    Task UpdateAsync(Trip trip);
    Task DeleteAsync(int tripId);
}