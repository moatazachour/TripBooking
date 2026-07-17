using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Services.Interfaces;

public interface ITripActivityService
{
    Task<List<TripActivity>> GetActivitiesForTripAsync(int tripId);
    Task AddActivityToTripAsync(int tripId, int activityId, int quantity);
    Task UpdateQuantityAsync(int tripActivityId, int quantity, byte[] rowVersion);
    Task RemoveActivityFromTripAsync(int tripActivityId);
}