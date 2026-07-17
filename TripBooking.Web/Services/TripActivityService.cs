using TripBooking.Web.Models.Entities;
using TripBooking.Web.Repositories.Interfaces;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Services;

public class TripActivityService : ITripActivityService
{
    private readonly ITripActivityRepository _tripActivityRepository;

    public TripActivityService(ITripActivityRepository tripActivityRepository)
    {
        _tripActivityRepository = tripActivityRepository;
    }

    public async Task<List<TripActivity>> GetActivitiesForTripAsync(int tripId)
        => await _tripActivityRepository.GetByTripIdAsync(tripId);

    public async Task AddActivityToTripAsync(int tripId, int activityId, int quantity)
    {
        // A real business rule, sitting exactly where Chapter 5.4 said it
        // should: in the Service, because it's a plain sanity check that
        // doesn't need the database — but it's still genuinely a business
        // rule (you can't book zero or negative units of anything), not an
        // HTTP concern, so it belongs here rather than in the Controller.
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));
        }

        await _tripActivityRepository.InsertAsync(tripId, activityId, quantity);

        // The actual price-freezing happened inside the stored procedure's
        // transaction (section 6.4) — the Service doesn't need to repeat
        // that logic, it just orchestrates the call and enforces the rules
        // that live above the database layer.
    }

    public async Task UpdateQuantityAsync(int tripActivityId, int quantity, byte[] rowVersion)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));
        }

        await _tripActivityRepository.UpdateQuantityAsync(tripActivityId, quantity, rowVersion);
    }

    public async Task RemoveActivityFromTripAsync(int tripActivityId)
        => await _tripActivityRepository.DeleteAsync(tripActivityId);
}