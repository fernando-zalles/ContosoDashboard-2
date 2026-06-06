namespace ContosoDashboard.Services;

public interface IDocumentValidationService
{
    IReadOnlyCollection<string> AllowedCategories { get; }
    IReadOnlyCollection<string> AllowedExtensions { get; }
    IReadOnlyCollection<string> AllowedContentTypes { get; }
    bool IsValidExtension(string fileName);
    bool IsValidContentType(string contentType);
    bool IsValidFileSize(long fileSizeBytes, long maxSizeBytes);
    string NormalizeTags(string tags);
}

public class DocumentValidationService : IDocumentValidationService
{
    public IReadOnlyCollection<string> AllowedCategories { get; } = new[]
    {
        "Project Documents",
        "Team Resources",
        "Personal Files",
        "Reports",
        "Presentations",
        "Other"
    };

    public IReadOnlyCollection<string> AllowedExtensions { get; } = new[]
    {
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".txt",
        ".jpg",
        ".jpeg",
        ".png"
    };

    public IReadOnlyCollection<string> AllowedContentTypes { get; } = new[]
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "image/jpeg",
        "image/png"
    };

    public bool IsValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension != null && AllowedExtensions.Contains(extension);
    }

    public bool IsValidContentType(string contentType)
    {
        return !string.IsNullOrWhiteSpace(contentType) && AllowedContentTypes.Contains(contentType);
    }

    public bool IsValidFileSize(long fileSizeBytes, long maxSizeBytes)
    {
        return fileSizeBytes > 0 && fileSizeBytes <= maxSizeBytes;
    }

    public string NormalizeTags(string tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return string.Empty;
        }

        var normalized = tags
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        return string.Join(",", normalized);
    }
}
