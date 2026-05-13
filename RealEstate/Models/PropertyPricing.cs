using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models;

/// <summary>
/// Property Pricing - tracks price changes and markup by managers
/// </summary>
public class PropertyPricing
{
    [Key]
    public int PricingId { get; set; }
    public int PropertyId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal MarkupAmount { get; set; }
    public decimal FinalPrice { get; set; }
    public int SetBy { get; set; }  // ManagerId
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;

    // Navigation
    public Property? Property { get; set; }
    public ApplicationUser? Manager { get; set; }
}

/// <summary>
/// Commission Rules - defined by managers
/// </summary>
public class CommissionRule
{
    [Key]
    public int RuleId { get; set; }
    public int? ManagerId { get; set; }  // FK → Users
    
    // Commission calculation settings
    public decimal CommissionPercent { get; set; } = 5m;  // 5% of sale price (overall commission rate)
    public decimal AgentSplitPercent { get; set; } = 40m;  // 40% of commission pool goes to agent
    public decimal CompanySplitPercent { get; set; } = 60m;  // 60% of commission pool goes to company/broker
    
    // Backward compatibility - maps to CompanySplitPercent
    public decimal CompanySharePercent 
    { 
        get => CompanySplitPercent; 
        set => CompanySplitPercent = value; 
    }
    
    public decimal MinimumDealThreshold { get; set; } = 50000m;  // Minimum to qualify for payout
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ApplicationUser? Manager { get; set; }
}
