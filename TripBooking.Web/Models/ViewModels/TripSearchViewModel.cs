using TripBooking.Web.Models.Entities;

namespace TripBooking.Web.Models.ViewModels;

public class TripSearchViewModel
{
    // ----- What the user searched for (also re-populates the form) -----
    public string? DestinationFilter { get; set; }
    public string? StatusFilter { get; set; }
    public string SortColumn { get; set; } = "CreatedAtUtc";
    public string SortDirection { get; set; } = "DESC";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // ----- What we found -----
    public List<Trip> Results { get; set; } = new();
    public int TotalCount { get; set; }

    // A couple of small computed helpers the View will lean on heavily —
    // notice these aren't stored, they're calculated on the fly from the
    // properties above. Keeping this math here instead of duplicating it
    // inside the .cshtml (Razor) file is a good habit: it's testable C#,
    // not buried in markup.
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}