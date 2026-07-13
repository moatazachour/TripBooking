using Microsoft.AspNetCore.Mvc;
using TripBooking.Web.Models.Entities;
using TripBooking.Web.Models.ViewModels;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Controllers;

public class ActivitiesController : Controller
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    // GET /Activities
    public async Task<IActionResult> Index()
    {
        var activities = await _activityService.GetAllActivitiesAsync();
        return View(activities);
    }

    // GET /Activities/Create
    // Displays an empty form. No [HttpGet] attribute is strictly required
    // here — GET is the default verb for any action with no [HttpX]
    // attribute at all — but we write it explicitly for clarity, which is
    // the convention this course follows throughout so the verb is never
    // ambiguous at a glance.
    [HttpGet]
    public IActionResult Create()
    {
        // A fresh, empty ViewModel — IsActive defaults to true per the
        // ViewModel's own default, everything else is blank.
        var viewModel = new ActivityFormViewModel();
        return View(viewModel);
    }

    // POST /Activities/Create
    // Receives the submitted form. Same action NAME as the GET above
    // ("Create"), but a different HTTP VERB — this is how MVC tells them
    // apart; two C# methods cannot otherwise share a name, so [HttpPost]
    // and [HttpGet] (or its absence) are what make this legal C#.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityFormViewModel viewModel)
    {
        // ModelState is populated automatically by MVC's model binder BEFORE
        // this method body even starts running. It checked every Data
        // Annotation attribute on ActivityFormViewModel (Required, StringLength,
        // Range) against the submitted form values. We MUST check it — MVC
        // does not block the request automatically just because validation
        // failed; that's a deliberate design decision (explained fully in
        // Ch 5) so you always get a chance to add custom logic first.
        if (!ModelState.IsValid)
        {
            // Re-render the SAME view, passing back the SAME viewModel, so
            // the user sees their own input still filled in, plus validation
            // error messages (rendered by <span asp-validation-for="..."> —
            // explained in the View section below). We do NOT redirect here:
            // a redirect would lose the ModelState errors and the user's
            // typed input, since a redirect is a brand new GET request.
            return View(viewModel);
        }

        // Map ViewModel -> Entity. This is intentionally explicit, not
        // automatic — a mapping library (like AutoMapper) can do this for
        // you in larger projects, but seeing it written out once, by hand,
        // is worth it for understanding exactly what crosses the boundary
        // between "what the form submitted" and "what gets persisted."
        var activity = new Activity
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            UnitPrice = viewModel.UnitPrice,
            IsActive = viewModel.IsActive
        };

        await _activityService.CreateActivityAsync(activity);

        // RedirectToAction, not View(). This is the Post-Redirect-Get (PRG)
        // pattern — explained in detail right after this code block, because
        // it's important enough to deserve its own explanation.
        return RedirectToAction(nameof(Index));
    }

    // GET /Activities/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)   
    {
        var activity = await _activityService.GetActivityByIdAsync(id);

        if (activity is null)
        {
            return NotFound();  // renders a 404 — a real IActionResult, not an exception
        }

        var viewModel = new ActivityFormViewModel
        {
            ActivityId = activity.ActivityId,
            Name = activity.Name,
            Description = activity.Description,
            UnitPrice = activity.UnitPrice,
            IsActive = activity.IsActive
        };

        return View(viewModel);
    }

    // POST /Activities/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActivityFormViewModel viewModel)
    {
        if (id != viewModel.ActivityId)
        {
            // Defense in depth: the route's {id} and the form's hidden
            // ActivityId field should always match. If they don't, something
            // is wrong (tampering, or a bug) — refuse rather than guess.
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var activity = new Activity
        {
            ActivityId = viewModel.ActivityId,
            Name = viewModel.Name,
            Description = viewModel.Description,
            UnitPrice = viewModel.UnitPrice,
            IsActive = viewModel.IsActive
        };

        await _activityService.UpdateActivityAsync(activity);

        return RedirectToAction(nameof(Index));
    }

    // GET /Activities/Delete/5
    // Shows a confirmation page FIRST — we never delete on a GET request.
    // This matters for a real reason, not just convention: GET requests are
    // supposed to be "safe" (no side effects) per the HTTP spec, and browsers,
    // crawlers, and link-prefetching can all trigger a GET without the user
    // meaning to. A GET that deletes data is a real, documented vulnerability
    // pattern from the early 2000s web that MVC's conventions steer you away from.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var activity = await _activityService.GetActivityByIdAsync(id);

        if (activity is null)
        {
            return NotFound();
        }

        return View(activity);  // note: the entity itself is fine here, it's read-only display
    }

    // POST /Activities/Delete/5
    // The actual delete happens only here, on POST, after confirmation.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _activityService.DeleteActivityAsync(id);
        return RedirectToAction(nameof(Index));
    }
}