using TripBooking.Web.Models.Entities;
using TripBooking.Web.Repositories.Interfaces;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;

    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<List<Activity>> GetAllActivitiesAsync()
    {
        // Thin pass-through today — same honest note as before: the value
        // of this layer shows up once real rules arrive (Ch 5–6), not here.
        return await _activityRepository.GetAllAsync();
    }
}
