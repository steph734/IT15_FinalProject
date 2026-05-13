using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

/// <summary>
/// Financial Record - comprehensive financial tracking
/// </summary>
public class FinancialRecord
{
    [Key]
    public int RecordId { get; set; }
    public int? TransactionId { get; set; }  // FK (optional)
    public string Type { get; set; } = string.Empty; // Revenue, Commission, Expense, Refund
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty; // Property Sale, Agent Commission, Marketing, etc.
    public string Description { get; set; } = string.Empty;
    public int? RecordedBy { get; set; }  // UserId (Accounting)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ReferenceNumber { get; set; } = string.Empty;

    // Navigation
    public Transaction? Transaction { get; set; }
    public ApplicationUser? RecordedByUser { get; set; }
}

/// <summary>
/// Audit Log - track all important actions in the system
/// </summary>
public class AuditLog
{
    [Key]
    public int LogId { get; set; }
    public int? UserId { get; set; }  // FK → Users
    public string UserRole { get; set; } = string.Empty; // Seller, Manager, Accounting, Agent, etc.
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Approve, Reject, etc.
    public string EntityType { get; set; } = string.Empty; // Property, Transaction, Commission, etc.
    public int? EntityId { get; set; }  // ID of the affected entity
    public string Description { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string OldValues { get; set; } = string.Empty;  // JSON
    public string NewValues { get; set; } = string.Empty;  // JSON

    // Navigation
    public ApplicationUser? User { get; set; }
}
