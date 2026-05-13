namespace RealEstate.Models;

/// <summary>
/// Customer Inquiries about properties
/// </summary>
public class Inquiry
{
    public int InquiryId { get; set; }
    public int PropertyId { get; set; }
    public int? CustomerId { get; set; }  // FK → Users (can be null for guest inquiries)
    public int? AgentId { get; set; }  // FK → Users (assigned agent)
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Responded, Closed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public string Response { get; set; } = string.Empty;

    // Navigation
    public Property? Property { get; set; }
    public ApplicationUser? Customer { get; set; }
    public ApplicationUser? Agent { get; set; }
}
