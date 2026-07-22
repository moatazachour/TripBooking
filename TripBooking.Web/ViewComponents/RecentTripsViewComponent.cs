using Microsoft.AspNetCore.Mvc;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.ViewComponents;

// The "ViewComponent" suffix is another naming convention — MVC strips it
// off the same way it strips "Controller" off a Controller's class name,
// which is exactly why we'll invoke this one as just "RecentTrips" below.
public class RecentTripsViewComponent : ViewComponent
{
    private readonly ITripService _tripService;

    public RecentTripsViewComponent(ITripService tripService)
    {
        _tripService = tripService;
    }

    // InvokeAsync is to a View Component what an action method is to a
    // Controller — it's the method that actually runs when this component
    // is rendered, and it returns a View, exactly like a Controller action
    // does. The name InvokeAsync is itself a required convention, similar
    // to how Index is the conventional default action name.
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var allTrips = await _tripService.GetAllTripsAsync();
        var recentTrips = allTrips.Take(3).ToList();

        return View(recentTrips);
    }
}