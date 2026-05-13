using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models;

public class CommissionDeal
{
    [Key]
    public int Id { get; set; }

    public int? PropertyId { get; set; }

    [MaxLength(200)]
    public string PropertyLabel { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? PropertyAddress { get; set; }

    public int AgentUserId { get; set; }

    public int? BrokerUserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossCommission { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal AgentSplitPercent { get; set; } = 70m;

    [Column(TypeName = "decimal(5,2)")]
    public decimal CompanySplitPercent { get; set; } = 30m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AddOnsTotal { get; set; } = 0m; // deductions or fees applied by manager

    [Column(TypeName = "decimal(18,2)")]
    public decimal AgentPayoutAmount { get; set; } = 0m;

    [MaxLength(1000)]
    public string DealDocumentsJson { get; set; } = "[]"; // list of doc URLs / names

    public CommissionDealStatus Status { get; set; } = CommissionDealStatus.PendingManagerApproval;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ManagerApprovedAtUtc { get; set; }
    public int? ManagerApprovedByUserId { get; set; }

    [MaxLength(500)]
    public string? ManagerNotes { get; set; }

    [MaxLength(80)]
    public string? BatchId { get; set; }

    public DateTime? TransferredToAccountingAtUtc { get; set; }

    public AccountingPayoutStatus AccountingPayoutStatus { get; set; } = AccountingPayoutStatus.AwaitingReview;

    public DateTime? AccountingReleasedAtUtc { get; set; }
    public int? AccountingReleasedByUserId { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public DateTime? RejectedAtUtc { get; set; }
    public int? RejectedByUserId { get; set; }

    [MaxLength(500)]
    public string? PayStubUrl { get; set; }

    [MaxLength(2000)]
    public string AuditJson { get; set; } = "{}";
}

