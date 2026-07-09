namespace TripBooking.Web.Models.Entities;

public class Trip
{
    public int TripId { get; set; }
    public int TravelerId { get; set; }
    public string Destination { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Budget { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? DocumentPath { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // No navigation properties like EF Core would generate — with ADO.NET,
    // if a screen needs the Traveler's name alongside a Trip, the stored
    // procedure joins and returns it as flat columns (see Trip_GetAll),
    // and we add plain properties for that here rather than pretending
    // there's a lazy-loaded object graph:
    public string? TravelerFirstName { get; set; }
    public string? TravelerLastName { get; set; }
}
