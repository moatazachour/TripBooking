using Microsoft.AspNetCore.Mvc;
using TripBooking.Web.Models.Entities;
using TripBooking.Web.Models.ViewModels;
using TripBooking.Web.Repositories;
using TripBooking.Web.Services;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Controllers
{
    public class TripsController : Controller
    {
        private readonly ITripService _tripService;
        private readonly ITripActivityService _tripActivityService;
        private readonly IActivityService _activityService;
        private readonly IFileStorageService _fileStorageService;

        public TripsController(
            ITripService tripService,
            ITripActivityService tripActivityService,
            IActivityService activityService,
            IFileStorageService fileStorageService)
        {
            _tripService = tripService;
            _tripActivityService = tripActivityService;
            _activityService = activityService;
            _fileStorageService = fileStorageService;
        }


        // GET /Trips?destinationFilter=Djerba&sortColumn=Budget&sortDirection=ASC&pageNumber=1
        [HttpGet]
        public async Task<IActionResult> Index(TripSearchViewModel searchViewModel)
        {
            // MVC's model binder already populated searchViewModel's filter/sort/paging
            // properties directly from the query string, using the same name-matching
            // rules from Chapter 4 (query string key "destinationFilter" -> property
            // "DestinationFilter", case-insensitively). No manual Request.Query[...]
            // reading required anywhere.

            var (trips, totalCount) = await _tripService.SearchTripsAsync(
                searchViewModel.DestinationFilter,
                searchViewModel.StatusFilter,
                searchViewModel.SortColumn,
                searchViewModel.SortDirection,
                searchViewModel.PageNumber,
                searchViewModel.PageSize);

            searchViewModel.Results = trips;
            searchViewModel.TotalCount = totalCount;

            return View(searchViewModel);
        }
        
        
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TripFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TripFormViewModel viewModel, IFormFile? documentFile)
        {
            // Cross-field rule: Data Annotations can't express "EndDate >= StartDate"
            // because it depends on two properties at once. We check it explicitly
            // and add the failure to ModelState ourselves, using AddModelError.
            if (viewModel.EndDate < viewModel.StartDate)
            {
                ModelState.AddModelError(
                    key: nameof(viewModel.EndDate),
                    errorMessage: "End date must be on or after the start date.");
            }

            // IMPORTANT: this check happens AFTER the automatic Data Annotation
            // validation already ran (per 5.1), and BEFORE we check ModelState.IsValid
            // below. This ordering is what lets a single ModelState.IsValid check
            // catch both kinds of errors together.
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // NOTE: hardcoding TravelerId here is a placeholder — once Part 3
            // (JWT auth) exists, this comes from the logged-in user's identity,
            // never from a form field a user could tamper with. Flagged now so
            // it isn't a surprise later; this is a deliberate, temporary stand-in.
            var trip = new Trip
            {
                TravelerId = 3, // seeded Moataz traveler from our schema script
                Destination = viewModel.Destination,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Budget = viewModel.Budget,
                Status = "Draft"
            };

            var createdTrip = await _tripService.CreateTripAsync(trip);

            // The file needs the new TripId (for naming), so it's saved AFTER the
            // Trip itself already exists — order matters here.
            if (documentFile is not null && documentFile.Length > 0)
            {
                try
                {
                    var storedFileName = await _fileStorageService.SaveTripDocumentAsync(createdTrip.TripId, documentFile);

                    // createdTrip came straight back from Trip_Insert's own SELECT
                    // (see Chapter 6's updated Repository), so createdTrip.RowVersion
                    // is the REAL value SQL Server just assigned — not a C# default
                    // empty array. That's exactly what UpdateAsync's optimistic-
                    // concurrency WHERE RowVersion = @RowVersion check needs to
                    // succeed on the very first try, right after Insert.
                    createdTrip.DocumentPath = storedFileName;
                    await _tripService.UpdateTripAsync(createdTrip);
                }
                catch (ArgumentException ex)
                {
                    TempData["ErrorMessage"] = $"Trip was created, but the file upload failed: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Details), new { id = createdTrip.TripId });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip?.DocumentPath is null)
            {
                return NotFound();
            }

            var document = await _fileStorageService.GetTripDocumentAsync(trip.DocumentPath);
            if (document is null)
            {
                return NotFound();
            }

            // File(...) is another IActionResult, alongside View(), Redirect(), and
            // NotFound() from earlier chapters — this one streams raw bytes back
            // with a specific Content-Type, which is what makes the browser either
            // display it inline (PDFs/images often do) or offer a download,
            // depending on the browser and the Content-Disposition behavior.
            return File(document.Value.Content, document.Value.ContentType, document.Value.FileName);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip is null)
            {
                return NotFound();
            }

            var tripActivities = await _tripActivityService.GetActivitiesForTripAsync(id);
            var availableActivities = await _activityService.GetAllActivitiesAsync();

            var viewModel = new TripDetailsViewModel
            {
                Trip = trip,
                BookedActivities = tripActivities,
                AvailableActivities = availableActivities.Where(a => a.IsActive).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(int tripId, int activityId, int quantity)
        {
            try
            {
                await _tripActivityService.AddActivityToTripAsync(tripId, activityId, quantity);
            }
            catch (InvalidOperationException ex)
            {
                // The "Activity not found or not active" translated exception
                // from section 6.5, surfaced as a friendly message via TempData
                // (a way to pass a one-time message across a redirect — briefly
                // explained right after this code block).
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveActivity(int tripActivityId, int tripId)
        {
            await _tripActivityService.RemoveActivityFromTripAsync(tripActivityId);
            return RedirectToAction(nameof(Details), new { id = tripId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip is null)
            {
                return NotFound();
            }

            var viewModel = new TripFormViewModel
            {
                TripId = trip.TripId,
                Destination = trip.Destination,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Budget = trip.Budget,
                RowVersion = trip.RowVersion  // carried through as a hidden field, section 6.10
            };

            return View(viewModel);
        }

        // POST /Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TripFormViewModel viewModel)
        {
            if (id != viewModel.TripId)
            {
                return BadRequest();
            }

            if (viewModel.EndDate < viewModel.StartDate)
            {
                ModelState.AddModelError(nameof(viewModel.EndDate), "End date must be on or after the start date.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var trip = new Trip
            {
                TripId = viewModel.TripId,
                TravelerId = 3, // see the note in Create — replaced properly in Part 3
                Destination = viewModel.Destination,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Budget = viewModel.Budget,
                Status = "Draft",
                RowVersion = viewModel.RowVersion,
                DocumentPath = viewModel.DocumentPath
            };

            try
            {
                await _tripService.UpdateTripAsync(trip);
            }
            catch (DbConcurrencyException ex)
            {
                // This is the payoff of the whole optimistic-concurrency story
                // from 6.6: instead of a crash or a silent overwrite, the user
                // gets a specific, honest, actionable message.
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Details), new { id = viewModel.TripId });
        }
    }
}
