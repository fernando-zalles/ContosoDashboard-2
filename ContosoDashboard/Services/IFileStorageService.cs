namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string relativePath, string contentType);
    Task<Stream> DownloadAsync(string relativePath);
    Task<bool> DeleteAsync(string relativePath);
}
