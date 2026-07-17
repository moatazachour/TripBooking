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

    public async Task<int> CreateTripAsync(Trip trip) => await _tripRepository.InsertAsync(trip);

    public async Task UpdateTripAsync(Trip trip) => await _tripRepository.UpdateAsync(trip);

    public async Task DeleteTripAsync(int tripId) => await _tripRepository.DeleteAsync(tripId);
}