namespace TripBooking.Web.Services;

using TripBooking.Web.Services.Interfaces;

public class FileStorageService : IFileStorageService
{
    // Deliberately a short, fixed allow-list — not a deny-list. Explained
    // in 8.4: allow-lists are the only safe way to gate file types.
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly string _storageRootPath;

    public FileStorageService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configuredPath = configuration["FileStorage:TripDocumentsPath"]
            ?? throw new InvalidOperationException("FileStorage:TripDocumentsPath is not configured.");

        // ContentRootPath is the app's own root folder (where the .csproj
        // lives) — NOT WebRootPath (which is specifically wwwroot). Using
        // ContentRootPath here is what physically keeps this folder outside
        // the publicly-servable wwwroot tree from section 8.2.
        _storageRootPath = Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredPath));

        if (!Directory.Exists(_storageRootPath))
        {
            Directory.CreateDirectory(_storageRootPath);
        }
    }

    public async Task<string> SaveTripDocumentAsync(int tripId, IFormFile file)
    {
        // Guard 1: reject empty or absurdly large files. Checking Length
        // here, in our own code, matters even though ASP.NET Core also has
        // a server-wide request size limit — that limit exists to protect
        // the whole SERVER from a giant request; this check exists to
        // protect OUR feature's specific, sane expectations (nobody's trip
        // itinerary is 200MB), and gives a much friendlier error message
        // than a generic "413 Request Entity Too Large" would.
        if (file.Length == 0)
        {
            throw new ArgumentException("The uploaded file is empty.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new ArgumentException("The uploaded file exceeds the 5 MB limit.");
        }

        // Guard 2: validate the extension against an ALLOW-list. Explained
        // fully in 8.4 — this is the single most important line in this
        // entire chapter.
        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Only PDF, JPG, and PNG files are allowed.");
        }

        // Guard 3: NEVER use the uploaded file's own name to build a path
        // on our server. We generate a brand-new, random name ourselves.
        // Full explanation in 8.4 — this single decision closes off an
        // entire, well-known class of attack.
        var storedFileName = $"{tripId}_{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_storageRootPath, storedFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return storedFileName;
    }

    public async Task<(byte[] Content, string ContentType, string FileName)?> GetTripDocumentAsync(string storedFileName)
    {
        // Guard 4: re-validate the incoming filename before touching the
        // filesystem with it — see 8.4's path traversal discussion. Even
        // though storedFileName is supposed to only ever be a value WE
        // generated and put in the database ourselves, defense in depth
        // means never trusting a filesystem path built from any external
        // input, including "our own" data round-tripped through a database
        // and a URL.
        if (storedFileName.Contains("..") || Path.IsPathRooted(storedFileName))
        {
            throw new ArgumentException("Invalid file reference.");
        }

        var fullPath = Path.Combine(_storageRootPath, storedFileName);

        // Guard 5: after combining, confirm the resulting path is STILL
        // inside our storage root. This catches sneaky traversal attempts
        // that guard 4's simple ".." string check might not, because
        // Path.Combine + Path.GetFullPath together fully resolve any
        // remaining relative segments before we compare.
        var normalizedFullPath = Path.GetFullPath(fullPath);
        if (!normalizedFullPath.StartsWith(_storageRootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid file reference.");
        }

        if (!File.Exists(normalizedFullPath))
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(normalizedFullPath);
        var contentType = GetContentType(Path.GetExtension(normalizedFullPath));

        return (content, contentType, storedFileName);
    }

    public void DeleteTripDocument(string storedFileName)
    {
        var fullPath = Path.Combine(_storageRootPath, storedFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
    };
}