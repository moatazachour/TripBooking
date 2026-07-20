using Microsoft.AspNetCore.Mvc;
using TripBooking.Web.Services.Interfaces;

namespace TripBooking.Web.Controllers;

public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET /Reports/RevenueByActivity
    [HttpGet]
    public async Task<IActionResult> RevenueByActivity()
    {
        var rows = await _reportService.GetRevenueByActivityAsync();
        return View(rows);
    }

    // GET /Reports/TripsByDestination?year=2026
    [HttpGet]
    public async Task<IActionResult> TripsByDestination(int? year)
    {
        var rows = await _reportService.GetTripsByDestinationAsync(year);
        ViewData["SelectedYear"] = year;
        return View(rows);
    }

    // GET /Reports/ExportRevenueByActivityCsv
    [HttpGet]
    public async Task<IActionResult> ExportRevenueByActivityCsv()
    {
        var rows = await _reportService.GetRevenueByActivityAsync();

        var csvBuilder = new System.Text.StringBuilder();
        csvBuilder.AppendLine("Activity,Total Revenue,Units Booked,Trips");

        foreach (var row in rows)
        {
            // Wrapping Name in quotes handles the case where an Activity's name
            // itself contains a comma (e.g. "City Tour, Extended") — without
            // quoting, that comma would be misread as a column separator by
            // Excel or any other CSV reader, silently corrupting the file.
            csvBuilder.AppendLine($"\"{row.Name}\",{row.TotalRevenue},{row.TotalUnitsBooked},{row.TripCount}");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());

        // File(...) again — same IActionResult from Chapter 8's document download,
        // just with a Content-Type that tells the browser "this is a spreadsheet,"
        // and a suggested filename via the third argument.
        return File(bytes, "text/csv", "ActivityRevenue.csv");
    }
}   