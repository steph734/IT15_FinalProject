using RealEstate.Models;
using System.Text.Json;

namespace RealEstate.Services
{
    /// <summary>
    /// Service for interacting with the RentCast API
    /// Provides property valuation and market data
    /// </summary>
    public class RentCastService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RentCastService> _logger;

        public RentCastService(HttpClient client, IConfiguration configuration, ILogger<RentCastService> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;

            // Configure API key from appsettings or environment variable
            var apiKey = _configuration["RentCast:ApiKey"]
                ?? Environment.GetEnvironmentVariable("RentCastKey");

            if (!string.IsNullOrEmpty(apiKey))
            {
                _client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            }

            // Set base address for RentCast API
            _client.BaseAddress = new Uri("https://api.rentcast.io/");
        }

        /// <summary>
        /// Get property valuation (AVM - Automated Valuation Model) for an address
        /// </summary>
        /// <param name="address">Full property address</param>
        /// <returns>RentCastValuation with market value and rent estimate</returns>
        public async Task<RentCastValuation?> GetValuationAsync(string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    _logger.LogWarning("Address is empty or null");
                    return null;
                }

                // Build the API URL with the address
                var encodedAddress = Uri.EscapeDataString(address);
                var url = $"/v1/avm/value?address={encodedAddress}&propertyType=House&bedrooms=3&bathrooms=2&squareFootage=1500&compCount=5";

                _logger.LogInformation("Fetching RentCast valuation for address: {Address}", address);

                var response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var valuation = JsonSerializer.Deserialize<RentCastValuation>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (valuation != null)
                    {
                        _logger.LogInformation(
                            "RentCast valuation received: Market Value=${MarketValue}, Rent=${RentEstimate}, Cap Rate={CapRate}%",
                            valuation.MarketValue,
                            valuation.RentEstimate,
                            valuation.CalculateCapRate()
                        );
                    }

                    return valuation;
                }
                else
                {
                    _logger.LogWarning(
                        "RentCast API returned non-success status code: {StatusCode} for address: {Address}",
                        response.StatusCode,
                        address
                    );
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while calling RentCast API for address: {Address}", address);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed for RentCast API response");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching RentCast valuation for address: {Address}", address);
                return null;
            }
        }

        /// <summary>
        /// Get comparable properties for an address
        /// </summary>
        /// <param name="address">Full property address</param>
        /// <param name="count">Number of comparables to return (max 20)</param>
        /// <returns>List of comparable properties</returns>
        public async Task<List<ComparableProperty>?> GetComparablesAsync(string address, int count = 5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                    return null;

                var encodedAddress = Uri.EscapeDataString(address);
                var url = $"/v1/avm/value?address={encodedAddress}&compCount={Math.Min(count, 20)}";

                var response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var valuation = JsonSerializer.Deserialize<RentCastValuation>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return valuation?.Comparables;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comparables from RentCast API");
                return null;
            }
        }

        /// <summary>
        /// Calculate yield score (cap rate) for a property
        /// </summary>
        /// <param name="marketValue">Property market value</param>
        /// <param name="monthlyRent">Estimated monthly rent</param>
        /// <returns>Cap rate as percentage</returns>
        public decimal CalculateYieldScore(decimal marketValue, decimal monthlyRent)
        {
            if (marketValue <= 0) return 0;

            // Cap Rate Formula: (Annual Net Operating Income / Market Value) * 100
            // Assuming 30% expense ratio (property management, maintenance, vacancy, insurance, taxes)
            decimal annualGrossRent = monthlyRent * 12;
            decimal annualNetRent = annualGrossRent * 0.70m; // 70% of gross = net after 30% expenses
            decimal capRate = (annualNetRent / marketValue) * 100;

            return Math.Round(capRate, 2);
        }

        /// <summary>
        /// Get profitability rating based on cap rate
        /// </summary>
        /// <param name="capRate">Capitalization rate percentage</param>
        /// <returns>Profitability rating string</returns>
        public string GetProfitabilityRating(decimal capRate)
        {
            return capRate switch
            {
                >= 8 => "Excellent - High Yield Investment",
                >= 6 => "Good - Solid Investment",
                >= 4 => "Fair - Moderate Returns",
                _ => "Poor - Low Returns"
            };
        }

        /// <summary>
        /// Check if the API is properly configured with an API key
        /// </summary>
        public bool IsConfigured()
        {
            var apiKey = _configuration["RentCast:ApiKey"]
                ?? Environment.GetEnvironmentVariable("RentCastKey");

            return !string.IsNullOrEmpty(apiKey);
        }
    }
}
