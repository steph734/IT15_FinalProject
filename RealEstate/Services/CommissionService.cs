namespace RealEstate.Services;

/// <summary>
/// Commission Service - handles commission calculations and splits
/// </summary>
public class CommissionService
{
    /// <summary>
    /// Calculates the 5% commission pool from the final sale price
    /// </summary>
    public decimal CalculateCommissionPool(decimal finalPrice, decimal commissionRate = 0.05m)
    {
        return finalPrice * commissionRate;
    }
    
    /// <summary>
    /// Calculates the 60/40 split between Broker and Agent
    /// </summary>
    public (decimal brokerAmount, decimal agentAmount) CalculateSplit(decimal pool, decimal brokerPercent = 0.60m, decimal agentPercent = 0.40m)
    {
        decimal brokerAmount = pool * brokerPercent;
        decimal agentAmount = pool * agentPercent;
        
        return (brokerAmount, agentAmount);
    }
    
    /// <summary>
    /// Full commission calculation for a sale
    /// </summary>
    public CommissionCalculationResult CalculateSaleCommission(decimal finalPrice, decimal commissionRate = 0.05m, decimal brokerSplitPercent = 0.60m, decimal agentSplitPercent = 0.40m)
    {
        var pool = CalculateCommissionPool(finalPrice, commissionRate);
        var (brokerEarnings, agentEarnings) = CalculateSplit(pool, brokerSplitPercent, agentSplitPercent);
        
        return new CommissionCalculationResult
        {
            TotalContractPrice = finalPrice,
            CommissionPool = pool,
            BrokerEarnings = brokerEarnings,
            AgentEarnings = agentEarnings,
            BrokerSplitPercent = brokerSplitPercent * 100,
            AgentSplitPercent = agentSplitPercent * 100,
            CommissionRate = commissionRate * 100
        };
    }
}

public class CommissionCalculationResult
{
    public decimal TotalContractPrice { get; set; }
    public decimal CommissionPool { get; set; }
    public decimal BrokerEarnings { get; set; }
    public decimal AgentEarnings { get; set; }
    public decimal BrokerSplitPercent { get; set; }
    public decimal AgentSplitPercent { get; set; }
    public decimal CommissionRate { get; set; }
}
