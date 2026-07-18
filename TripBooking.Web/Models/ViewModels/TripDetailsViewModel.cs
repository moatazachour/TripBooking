using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Models.ViewModels;

public class TripDetailsViewModel
{
    public Trip Trip { get; set; } = null!;
    public List<TripActivity> BookedActivities { get; set; } = new();
    public List<Activity> AvailableActivities { get; set; } = new();
}