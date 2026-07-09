using Microsoft.AspNetCore.Mvc;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Controllers;

public class ActivitiesController : Controller
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    public async Task<IActionResult> Index()
    {
        var activities = await _activityService.GetAllActivitiesAsync();
        return View(activities);
    }
}
