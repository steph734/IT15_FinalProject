using System;

namespace RealEstate.Models
{
    public class CreateCustomerDTO
    {
        // Personal Details
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string Address { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string ZipCode { get; set; }
        
        public string Country { get; set; }
        
        // Property Information
        public string PropertyType { get; set; }
        
        public string InterestedProperties { get; set; }
        
        public decimal? MinBudget { get; set; }
        
        public decimal? MaxBudget { get; set; }
        
        public string Status { get; set; }
        
        // Payment Information
        public string PaymentMethod { get; set; }
        
        public string CardholderName { get; set; }
        
        public string CardNumber { get; set; }
        
        public string ExpiryDate { get; set; }
        
        public string CVV { get; set; }
        
        public string Notes { get; set; }
        
        public bool AgreeTerms { get; set; }
    }
}
