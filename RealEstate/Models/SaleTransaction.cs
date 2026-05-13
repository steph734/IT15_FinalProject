using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models;

/// <summary>
/// Sale Transaction - tracks property sales and commission splits
/// </summary>
public class SaleTransaction
{
    [Key]
    public int SaleId { get; set; }
    
    public int PropertyId { get; set; }
    public Property? Property { get; set; }
    
    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }
    
    public int? ManagerId { get; set; }
    public ApplicationUser? Manager { get; set; }
    
    // Pricing
    public decimal TotalContractPrice { get; set; }  // Final sale price (Bought for)
    
    // Lease Term (for rentals only)
    public int LeaseTerm { get; set; } = 1;  // 1, 2, 3 years etc.
    
    // Commission Calculation (5% of TotalContractPrice)
    public decimal CommissionPool { get; set; }  // 5% of TotalContractPrice
    
    // Split (60% Broker / 40% Agent)
    public decimal BrokerEarnings { get; set; }  // 60% of CommissionPool
    public decimal AgentEarnings { get; set; }  // 40% of CommissionPool
    
    // Split percentages stored for record
    public decimal BrokerSplitPercent { get; set; } = 60m;
    public decimal AgentSplitPercent { get; set; } = 40m;
    public decimal CommissionRate { get; set; } = 5m;  // 5% standard
    
    // Status
    public string Status { get; set; } = "Pending";  // Pending, Approved, Paid
    
    // Dates
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PaidDate { get; set; }
    
    // Notes
    public string? Notes { get; set; }
}
