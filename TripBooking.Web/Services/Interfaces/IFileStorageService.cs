namespace TripBooking.Web.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveTripDocumentAsync(int tripId, IFormFile file);
    Task<(byte[] Content, string ContentType, string FileName)?> GetTripDocumentAsync(string storedFileName);
    void DeleteTripDocument(string storedFileName);
}