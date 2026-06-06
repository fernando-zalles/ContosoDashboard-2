using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Tags { get; set; }

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    [Required]
    public long FileSizeBytes { get; set; }

    [Required]
    public DateTime UploadDateUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public int UploadedById { get; set; }

    [ForeignKey("UploadedById")]
    public virtual User UploadedBy { get; set; } = null!;

    public int? ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    public bool IsShared { get; set; } = false;

    public virtual ICollection<DocumentShare> DocumentShares { get; set; } = new List<DocumentShare>();
}
