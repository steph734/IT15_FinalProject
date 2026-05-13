namespace RealEstate.Models;

public enum CommissionDealStatus
{
    PendingManagerApproval = 0,
    ApprovedForPayroll = 1,
    TransferredToAccounting = 2,
    Locked = 3,
    RejectedByAccounting = 4,
    Completed = 5
}

public enum AccountingPayoutStatus
{
    AwaitingReview = 0,
    Processing = 1,
    Released = 2
}

public enum CommissionBatchStatus
{
    AwaitingReview = 0,
    Processing = 1,
    Released = 2,
    Completed = 3
}
