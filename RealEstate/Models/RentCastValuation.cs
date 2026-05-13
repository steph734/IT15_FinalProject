using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Models
{
    /// <summary>
    /// Model for RentCast API AVM (Automated Valuation Model) response
    /// </summary>
    public class RentCastValuation
    {
        [JsonPropertyName("price")]
        public decimal MarketValue { get; set; }

        [JsonPropertyName("rent")]
        public decimal RentEstimate { get; set; }

        [JsonPropertyName("priceRangeLow")]
        public decimal PriceRangeLow { get; set; }

        [JsonPropertyName("priceRangeHigh")]
        public decimal PriceRangeHigh { get; set; }

        [JsonPropertyName("rentRangeLow")]
        public decimal RentRangeLow { get; set; }

        [JsonPropertyName("rentRangeHigh")]
        public decimal RentRangeHigh { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("comps")]
        public List<ComparableProperty>? Comparables { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Calculates the Capitalization Rate (Cap Rate) based on market value and rent estimate
        /// Formula: (Annual Net Rent / Market Value) * 100
        /// Assumes 30% expense ratio for property management, maintenance, vacancy, etc.
        /// </summary>
        public decimal CalculateCapRate()
        {
            if (MarketValue <= 0) return 0;

            decimal annualGrossRent = RentEstimate * 12;
            decimal annualNetRent = annualGrossRent * 0.70m; // 30% expense deduction
            decimal capRate = (annualNetRent / MarketValue) * 100;

            return Math.Round(capRate, 2);
        }

        /// <summary>
        /// Gets a profitability score based on the cap rate
        /// </summary>
        public string GetProfitabilityRating()
        {
            var capRate = CalculateCapRate();
            return capRate switch
            {
                >= 8 => "Excellent",
                >= 6 => "Good",
                >= 4 => "Fair",
                _ => "Poor"
            };
        }
    }

    /// <summary>
    /// Comparable property data from RentCast API
    /// </summary>
    public class ComparableProperty
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("bedrooms")]
        public int Bedrooms { get; set; }

        [JsonPropertyName("bathrooms")]
        public int Bathrooms { get; set; }

        [JsonPropertyName("squareFootage")]
        public int SquareFootage { get; set; }

        [JsonPropertyName("distance")]
        public double Distance { get; set; }
    }
}
