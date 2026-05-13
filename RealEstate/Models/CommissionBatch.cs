using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

public class CommissionBatch
{
    [Key]
    public int Id { get; set; }

    [MaxLength(80)]
    public string BatchId { get; set; } = string.Empty; // e.g. BATCH-2026-APR-24-001

    public int CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public CommissionBatchStatus Status { get; set; } = CommissionBatchStatus.AwaitingReview;

    public DateTime? CompletedAtUtc { get; set; }
    public int? CompletedByUserId { get; set; }
}

