namespace RealEstate.Models.Admin;

public class BrokerDeal
{
    public string TransactionId { get; set; } = "";

    public int PropertyId { get; set; }

    public string ClientName { get; set; } = "";

    public string? ClientEmail { get; set; }

    public string? ClientPhone { get; set; }

    public BrokerDealType DealType { get; set; }

    /// <summary>Contract or list value for sales; annualized rent total or key amount for demo.</summary>
    public decimal Amount { get; set; }

    public BrokerDealStatus Status { get; set; }

    public DateTime DealDate { get; set; }

    public BrokerPaymentStatus PaymentStatus { get; set; }

    public decimal CommissionAmount { get; set; }

    /// <summary>Broker commission rate applied for display (e.g. 0.025 for 2.5%).</summary>
    public decimal CommissionRate { get; set; }

    public BrokerCommissionPaidStatus CommissionPaid { get; set; }

    public BrokerPipelineStage PipelineStage { get; set; }

    public string CommissionNotes { get; set; } = "";
}
