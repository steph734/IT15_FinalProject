namespace RealEstate.Models;

public enum InquiryStatus { New, InProgress, Resolved }

public class PropertyInquiry
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int PropertyId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Reply { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime CreatedAt { get; set; }
}
