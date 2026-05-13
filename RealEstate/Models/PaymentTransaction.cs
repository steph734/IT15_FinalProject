using System;

namespace RealEstate.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string PayMongoPaymentIntentId { get; set; }

        public string PayMongoSourceId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "PHP";

        public string Status { get; set; } = "pending"; // pending, succeeded, failed

        public string PaymentMethod { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public string WebhookResponse { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsProcessed { get; set; } = false;
    }
}
