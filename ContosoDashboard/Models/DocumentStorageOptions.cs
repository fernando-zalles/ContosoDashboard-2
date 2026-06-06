namespace ContosoDashboard.Models;

public class DocumentStorageOptions
{
    public string RootPath { get; set; } = "AppData/uploads";
    public long MaxFileSizeBytes { get; set; } = 25 * 1024 * 1024;
}
