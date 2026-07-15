using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Repositories.Interfaces;

public interface ITripActivityRepository
{
    Task<List<TripActivity>> GetByTripIdAsync(int tripId);
    Task<int> InsertAsync(int tripId, int activityId, int quantity);
    Task UpdateQuantityAsync(int tripActivityId, int quantity, byte[] rowVersion);
    Task DeleteAsync(int tripActivityId);
}