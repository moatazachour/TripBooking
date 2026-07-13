using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Services.Interfaces;

public interface IActivityService
{
    Task<List<Activity>> GetAllActivitiesAsync();
    Task<Activity?> GetActivityByIdAsync(int activityId);
    Task CreateActivityAsync(Activity activity);
    Task UpdateActivityAsync(Activity activity);
    Task DeleteActivityAsync(int activityId);
}
