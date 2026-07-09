namespace TripBooking.Web.Models.Entities;

public class TripActivity
{
    public int TripActivityId { get; set; }
    public int TripId { get; set; }
    public int ActivityId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtBooking { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public string? ActivityName { get; set; }
    public decimal? CurrentUnitPrice { get; set; }
}
