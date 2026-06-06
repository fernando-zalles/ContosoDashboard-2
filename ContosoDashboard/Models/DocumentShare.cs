using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentShare
{
    [Key]
    public int DocumentShareId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; } = null!;

    public int? RecipientUserId { get; set; }

    [ForeignKey("RecipientUserId")]
    public virtual User? RecipientUser { get; set; }

    [MaxLength(100)]
    public string? RecipientTeam { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    [ForeignKey("SharedByUserId")]
    public virtual User SharedByUser { get; set; } = null!;

    [Required]
    public DateTime SharedOnUtc { get; set; } = DateTime.UtcNow;
}
