using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RealEstate.Services
{
    /// <summary>
    /// Geocoding service using LocationIQ API
    /// Converts Philippine addresses to Latitude/Longitude coordinates
    /// </summary>
    public class GeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeocodingService> _logger;
        private const string BaseUrl = "https://us1.locationiq.com/v1";

        public GeocodingService(HttpClient httpClient, IConfiguration configuration, ILogger<GeocodingService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["LocationIQ:ApiKey"] 
                ?? throw new ArgumentNullException("LocationIQ API key not configured");
            _logger = logger;
        }

        /// <summary>
        /// Geocode a Philippine address to get Latitude and Longitude
        /// </summary>
        public async Task<GeocodingResult> GeocodeAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            try
            {
                // Format the URL with address and API key
                string encodedAddress = Uri.EscapeDataString(address);
                string url = $"{BaseUrl}/search.php?key={_apiKey}&q={encodedAddress}&format=json&countrycodes=ph&limit=1";

                _logger.LogInformation("Geocoding address: {Address}", address);

                var response = await _httpClient.GetStringAsync(url);
                var results = JsonConvert.DeserializeObject<LocationIQResult[]>(response);

                if (results == null || results.Length == 0)
                {
                    _logger.LogWarning("No geocoding results found for: {Address}", address);
                    return null;
                }

                var result = results[0];

                return new GeocodingResult
                {
                    Latitude = double.Parse(result.Lat),
                    Longitude = double.Parse(result.Lon),
                    DisplayName = result.DisplayName,
                    Address = result.Address?.ToString() ?? address,
                    Confidence = result.Confidence ?? 0,
                    RawResponse = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error geocoding address: {Address}", address);
                return null;
            }
        }

        /// <summary>
        /// Reverse geocode - get address from coordinates
        /// Useful when seller drags the pin
        /// </summary>
        public async Task<string> ReverseGeocodeAsync(double latitude, double longitude)
        {
            try
            {
                string url = $"{BaseUrl}/reverse.php?key={_apiKey}&lat={latitude}&lon={longitude}&format=json";

                var response = await _httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<LocationIQReverseResult>(response);

                return result?.DisplayName ?? $"Coordinates: {latitude}, {longitude}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reverse geocoding coordinates: {Lat}, {Lon}", latitude, longitude);
                return $"Coordinates: {latitude}, {longitude}";
            }
        }

        /// <summary>
        /// Validate if an address can be geocoded
        /// </summary>
        public async Task<bool> IsValidAddressAsync(string address)
        {
            var result = await GeocodeAddressAsync(address);
            return result != null && result.Confidence > 5;
        }
    }

    // ==================== MODELS ====================

    public class GeocodingResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DisplayName { get; set; } = "";
        public string Address { get; set; } = "";
        public int Confidence { get; set; }
        public string RawResponse { get; set; } = "";
    }

    // LocationIQ API Response Models
    public class LocationIQResult
    {
        [JsonProperty("lat")]
        public string Lat { get; set; } = "";
        
        [JsonProperty("lon")]
        public string Lon { get; set; } = "";
        
        [JsonProperty("display_name")]
        public string DisplayName { get; set; } = "";
        
        [JsonProperty("confidence")]
        public int? Confidence { get; set; }
        
        [JsonProperty("address")]
        public LocationIQAddress? Address { get; set; }
    }

    public class LocationIQAddress
    {
        [JsonProperty("road")]
        public string? Road { get; set; }
        
        [JsonProperty("neighbourhood")]
        public string? Neighbourhood { get; set; }
        
        [JsonProperty("suburb")]
        public string? Suburb { get; set; }
        
        [JsonProperty("city")]
        public string? City { get; set; }
        
        [JsonProperty("county")]
        public string? County { get; set; }
        
        [JsonProperty("state")]
        public string? State { get; set; }
        
        [JsonProperty("country")]
        public string? Country { get; set; }
        
        [JsonProperty("postcode")]
        public string? Postcode { get; set; }

        public override string ToString()
        {
            var parts = new[] { Road, Neighbourhood, City, State, Country };
            return string.Join(", ", Array.FindAll(parts, p => !string.IsNullOrWhiteSpace(p)));
        }
    }

    public class LocationIQReverseResult
    {
        [JsonProperty("display_name")]
        public string? DisplayName { get; set; }
        
        [JsonProperty("address")]
        public LocationIQAddress? Address { get; set; }
    }
}
