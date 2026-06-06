using ContosoDashboard.Models;
using Microsoft.Extensions.Options;

namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly DocumentStorageOptions _options;
    private readonly string _absoluteRoot;

    public LocalFileStorageService(IWebHostEnvironment environment, IOptions<DocumentStorageOptions> options)
    {
        _environment = environment;
        _options = options.Value;
        _absoluteRoot = Path.Combine(_environment.ContentRootPath, _options.RootPath);
        Directory.CreateDirectory(_absoluteRoot);
    }

    public async Task<string> UploadAsync(Stream fileStream, string relativePath, string contentType)
    {
        var fullPath = GetFullPath(relativePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        using var file = File.Create(fullPath);
        await fileStream.CopyToAsync(file);
        return relativePath.Replace("\\", "/");
    }

    public Task<Stream> DownloadAsync(string relativePath)
    {
        var fullPath = GetFullPath(relativePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Document not found", fullPath);
        }

        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public Task<bool> DeleteAsync(string relativePath)
    {
        var fullPath = GetFullPath(relativePath);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult(false);
        }

        File.Delete(fullPath);
        return Task.FromResult(true);
    }

    private string GetFullPath(string relativePath)
    {
        var sanitized = relativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Path.Combine(_absoluteRoot, sanitized.Replace("/", Path.DirectorySeparatorChar.ToString()));
    }
}
