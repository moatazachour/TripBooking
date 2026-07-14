using Microsoft.AspNetCore.Mvc;
using TripBooking.Web.Models.ViewModels;

namespace TripBooking.Web.Controllers
{
    public class TripsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TripFormViewModel viewModel)
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
            // ... proceed with mapping and saving, same pattern as Chapter 4 ...

            return View();
        }
    }
}
