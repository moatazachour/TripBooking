using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Repositories.Interfaces;

public interface IActivityRepository
{
    Task<List<Activity>> GetAllAsync();
    Task<Activity?> GetByIdAsync(int activityId);
}
