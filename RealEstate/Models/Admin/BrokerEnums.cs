namespace RealEstate.Models.Admin;

public enum BrokerDealType
{
    Sale,
    Rent
}

public enum BrokerDealStatus
{
    Pending,
    Completed
}

/// <summary>Client / deal payment status shown on transaction details.</summary>
public enum BrokerPaymentStatus
{
    Pending,
    Paid
}

public enum BrokerCommissionPaidStatus
{
    Unpaid,
    Paid
}

public enum BrokerPipelineStage
{
    Inquiry,
    Viewing,
    Negotiation,
    Closed
}
