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

    public async Task<Activity?> GetActivityByIdAsync(int activityId)
        => await _activityRepository.GetByIdAsync(activityId);

    public async Task CreateActivityAsync(Activity activity)
        => await _activityRepository.InsertAsync(activity);

    public async Task UpdateActivityAsync(Activity activity)
        => await _activityRepository.UpdateAsync(activity);

    public async Task DeleteActivityAsync(int activityId)
        => await _activityRepository.DeleteAsync(activityId);
}
