using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;
using Microsoft.Extensions.Options;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<DocumentUploadResult> UploadDocumentAsync(int userId, UploadDocumentRequest request);
    Task<IEnumerable<Document>> GetUserDocumentsAsync(int userId, DocumentQuery query);
    Task<IEnumerable<Document>> GetRecentDocumentsAsync(int userId, int limit = 5);
    Task<Document?> GetDocumentByIdAsync(int documentId);
}

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly IDocumentValidationService _validationService;
    private readonly DocumentStorageOptions _storageOptions;

    public DocumentService(
        ApplicationDbContext context,
        IFileStorageService storageService,
        IDocumentValidationService validationService,
        IOptions<DocumentStorageOptions> storageOptions)
    {
        _context = context;
        _storageService = storageService;
        _validationService = validationService;
        _storageOptions = storageOptions.Value;
    }

    public async Task<DocumentUploadResult> UploadDocumentAsync(int userId, UploadDocumentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return DocumentUploadResult.Failure("Document title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Category) || !_validationService.AllowedCategories.Contains(request.Category))
        {
            return DocumentUploadResult.Failure("Please select a valid document category.");
        }

        if (request.File == null)
        {
            return DocumentUploadResult.Failure("A document file is required.");
        }

        if (!_validationService.IsValidExtension(request.FileName))
        {
            return DocumentUploadResult.Failure("Unsupported file type.");
        }

        if (!_validationService.IsValidContentType(request.ContentType))
        {
            return DocumentUploadResult.Failure("Unsupported content type.");
        }

        if (!_validationService.IsValidFileSize(request.FileSizeBytes, _storageOptions.MaxFileSizeBytes))
        {
            return DocumentUploadResult.Failure($"Files must be {(_storageOptions.MaxFileSizeBytes / (1024 * 1024))} MB or smaller.");
        }

        var fileExtension = Path.GetExtension(request.FileName)?.ToLowerInvariant() ?? string.Empty;
        var storageFileName = $"{Guid.NewGuid()}{fileExtension}";
        var projectSegment = request.ProjectId.HasValue ? request.ProjectId.Value.ToString() : "personal";
        var relativePath = Path.Combine(userId.ToString(), projectSegment, storageFileName).Replace("\\", "/");

        try
        {
            await using var stream = request.File;
            await _storageService.UploadAsync(stream, relativePath, request.ContentType);
        }
        catch (Exception ex)
        {
            return DocumentUploadResult.Failure($"Unable to save file: {ex.Message}");
        }

        var document = new Document
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Category = request.Category,
            Tags = _validationService.NormalizeTags(request.Tags),
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            StoragePath = relativePath,
            FileSizeBytes = request.FileSizeBytes,
            UploadDateUtc = DateTime.UtcNow,
            UploadedById = userId,
            ProjectId = request.ProjectId
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return DocumentUploadResult.Success(document);
    }

    public async Task<IEnumerable<Document>> GetUserDocumentsAsync(int userId, DocumentQuery query)
    {
        var documents = _context.Documents
            .Include(d => d.UploadedBy)
            .Include(d => d.Project)
            .Where(d => d.UploadedById == userId);

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            documents = documents.Where(d => d.Category == query.Category);
        }

        if (query.ProjectId.HasValue)
        {
            documents = documents.Where(d => d.ProjectId == query.ProjectId.Value);
        }

        if (query.StartDateUtc.HasValue)
        {
            documents = documents.Where(d => d.UploadDateUtc >= query.StartDateUtc.Value);
        }

        if (query.EndDateUtc.HasValue)
        {
            documents = documents.Where(d => d.UploadDateUtc <= query.EndDateUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var search = query.SearchText.Trim().ToLowerInvariant();
            documents = documents.Where(d =>
                d.Title.ToLower().Contains(search) ||
                (d.Description != null && d.Description.ToLower().Contains(search)) ||
                (d.Tags != null && d.Tags.ToLower().Contains(search)) ||
                d.UploadedBy.DisplayName.ToLower().Contains(search) ||
                (d.Project != null && d.Project.Name.ToLower().Contains(search)));
        }

        documents = query.SortBy?.ToLowerInvariant() switch
        {
            "title" => documents.OrderBy(d => d.Title),
            "category" => documents.OrderBy(d => d.Category),
            "filesize" => documents.OrderBy(d => d.FileSizeBytes),
            _ => documents.OrderByDescending(d => d.UploadDateUtc)
        };

        return await documents.ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetRecentDocumentsAsync(int userId, int limit = 5)
    {
        return await _context.Documents
            .Where(d => d.UploadedById == userId)
            .OrderByDescending(d => d.UploadDateUtc)
            .Take(limit)
            .Include(d => d.Project)
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId)
    {
        return await _context.Documents
            .Include(d => d.UploadedBy)
            .Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId);
    }
}

public class UploadDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public string? Tags { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Stream File { get; set; } = Stream.Null;
    public long FileSizeBytes { get; set; }
}

public class DocumentUploadResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Document? Document { get; private set; }

    private DocumentUploadResult() { }

    public static DocumentUploadResult Success(Document document)
    {
        return new DocumentUploadResult { IsSuccess = true, Document = document };
    }

    public static DocumentUploadResult Failure(string message)
    {
        return new DocumentUploadResult { IsSuccess = false, ErrorMessage = message };
    }
}

public class DocumentQuery
{
    public string? SearchText { get; set; }
    public string? Category { get; set; }
    public int? ProjectId { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public string? SortBy { get; set; }
}
