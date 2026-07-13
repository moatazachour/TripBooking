using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Repositories.Interfaces;

public interface IActivityRepository
{
    Task<List<Activity>> GetAllAsync();
    Task<Activity?> GetByIdAsync(int activityId);
    Task<int> InsertAsync(Activity activity);
    Task UpdateAsync(Activity activity);
    Task DeleteAsync(int activityId);
}
