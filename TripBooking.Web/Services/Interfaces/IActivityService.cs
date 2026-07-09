using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Services.Interfaces;

public interface IActivityService
{
    Task<List<Activity>> GetAllActivitiesAsync();
}
