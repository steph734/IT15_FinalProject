using System;
using System.Collections.Generic;

namespace RealEstate.Models
{
    public class PaymentProcessRequest
    {
        public int CustomerId { get; set; }
    }

    public class PayMongoWebhookRequest
    {
        public PayMongoWebhookData Data { get; set; }
    }

    public class PayMongoWebhookData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public PayMongoWebhookAttributes Attributes { get; set; }
    }

    public class PayMongoWebhookAttributes
    {
        public string Status { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
