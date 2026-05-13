using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace RealEstate.Services
{
    public class WeatherForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/forecast";

        public WeatherForecastService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"] 
                ?? throw new ArgumentNullException("OpenWeatherMap API key not configured");
        }

        /// <summary>
        /// Gets 5-day/3-hour forecast for specific coordinates
        /// </summary>
        public async Task<WeatherForecastResponse> Get5DayForecastAsync(double lat, double lon)
        {
            string url = $"{BaseUrl}?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            
            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<WeatherForecastResponse>(response);
        }

        /// <summary>
        /// Gets weather forecast for a specific date/time
        /// </summary>
        public async Task<WeatherForecastItem> GetForecastForDateTimeAsync(double lat, double lon, DateTime targetDateTime)
        {
            var forecast = await Get5DayForecastAsync(lat, lon);
            
            if (forecast?.List == null || !forecast.List.Any())
                return null;

            // Find the forecast closest to the target date/time
            var closestForecast = forecast.List
                .OrderBy(f => Math.Abs((f.DateTime - targetDateTime).TotalHours))
                .FirstOrDefault();

            return closestForecast;
        }

        /// <summary>
        /// Analyzes weather and returns smart appointment insights
        /// </summary>
        public WeatherInsight AnalyzeWeatherForAppointment(WeatherForecastItem forecast)
        {
            if (forecast == null)
                return new WeatherInsight { Status = WeatherStatus.Unknown, Message = "Weather data unavailable" };

            var weather = forecast.Weather?.FirstOrDefault();
            var main = forecast.Main;

            var insight = new WeatherInsight
            {
                Temperature = main?.Temp ?? 0,
                FeelsLike = main?.FeelsLike ?? 0,
                Humidity = main?.Humidity ?? 0,
                WeatherDescription = weather?.Description ?? "Unknown",
                WeatherIcon = weather?.Icon ?? "",
                WindSpeed = forecast.Wind?.Speed ?? 0,
                ProbabilityOfRain = forecast.Pop * 100, // Convert to percentage
                Timestamp = forecast.DateTime
            };

            // Analyze conditions
            string mainWeather = weather?.Main?.ToLower() ?? "";
            
            if (mainWeather.Contains("rain") || mainWeather.Contains("drizzle") || mainWeather.Contains("thunderstorm"))
            {
                insight.Status = WeatherStatus.Rainy;
                insight.Alert = true;
                insight.AlertType = "rain";
                insight.Message = "Rainy weather detected. Inform agent to prepare umbrellas or offer to reschedule.";
                insight.Recommendation = "Consider rescheduling or prepare rain protection";
            }
            else if (mainWeather.Contains("thunderstorm"))
            {
                insight.Status = WeatherStatus.Stormy;
                insight.Alert = true;
                insight.AlertType = "storm";
                insight.Message = "Thunderstorm forecasted. Strongly recommend rescheduling for safety.";
                insight.Recommendation = "Recommend rescheduling appointment";
            }
            else if (main?.Temp > 33)
            {
                insight.Status = WeatherStatus.Hot;
                insight.Alert = true;
                insight.AlertType = "heat";
                insight.Message = "High heat expected. Remind agent to turn on house AC 15 mins before arrival.";
                insight.Recommendation = "Ensure property is cooled before visit";
            }
            else if (main?.Temp < 20)
            {
                insight.Status = WeatherStatus.Cool;
                insight.Alert = false;
                insight.Message = "Cool weather. Comfortable for property viewing.";
                insight.Recommendation = "Weather looks comfortable";
            }
            else if (mainWeather.Contains("clear") || mainWeather.Contains("clouds"))
            {
                insight.Status = WeatherStatus.Good;
                insight.Alert = false;
                insight.Message = "Weather looks good for a property tour!";
                insight.Recommendation = "Perfect weather for viewing";
            }
            else
            {
                insight.Status = WeatherStatus.Neutral;
                insight.Alert = false;
                insight.Message = "Weather conditions are acceptable.";
                insight.Recommendation = "Proceed with appointment";
            }

            return insight;
        }

        /// <summary>
        /// Get 8-hour forecast for appointment day (for display)
        /// </summary>
        public async Task<List<WeatherForecastItem>> GetDayForecastAsync(double lat, double lon, DateTime date)
        {
            var forecast = await Get5DayForecastAsync(lat, lon);
            
            if (forecast?.List == null)
                return new List<WeatherForecastItem>();

            // Filter forecasts for the specific date
            return forecast.List
                .Where(f => f.DateTime.Date == date.Date)
                .OrderBy(f => f.DateTime)
                .ToList();
        }
    }

    // ==================== MODELS ====================

    public class WeatherForecastResponse
    {
        [JsonProperty("cod")]
        public string Cod { get; set; }
        
        [JsonProperty("message")]
        public int Message { get; set; }
        
        [JsonProperty("cnt")]
        public int Count { get; set; }
        
        [JsonProperty("list")]
        public List<WeatherForecastItem> List { get; set; }
        
        [JsonProperty("city")]
        public CityInfo City { get; set; }
    }

    public class WeatherForecastItem
    {
        [JsonProperty("dt")]
        public long Dt { get; set; }
        
        [JsonProperty("dt_txt")]
        public string DtTxt { get; set; }
        
        [JsonIgnore]
        public DateTime DateTime => DateTime.Parse(DtTxt);
        
        [JsonProperty("main")]
        public MainInfo Main { get; set; }
        
        [JsonProperty("weather")]
        public List<WeatherInfo> Weather { get; set; }
        
        [JsonProperty("clouds")]
        public CloudsInfo Clouds { get; set; }
        
        [JsonProperty("wind")]
        public WindInfo Wind { get; set; }
        
        [JsonProperty("visibility")]
        public int Visibility { get; set; }
        
        [JsonProperty("pop")]
        public double Pop { get; set; } // Probability of precipitation
        
        [JsonProperty("rain")]
        public RainInfo Rain { get; set; }
    }

    public class MainInfo
    {
        [JsonProperty("temp")]
        public double Temp { get; set; }
        
        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }
        
        [JsonProperty("temp_min")]
        public double TempMin { get; set; }
        
        [JsonProperty("temp_max")]
        public double TempMax { get; set; }
        
        [JsonProperty("pressure")]
        public int Pressure { get; set; }
        
        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("main")]
        public string Main { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class CloudsInfo
    {
        [JsonProperty("all")]
        public int All { get; set; }
    }

    public class WindInfo
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
        
        [JsonProperty("deg")]
        public int Deg { get; set; }
        
        [JsonProperty("gust")]
        public double Gust { get; set; }
    }

    public class RainInfo
    {
        [JsonProperty("3h")]
        public double ThreeHours { get; set; }
    }

    public class CityInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("coord")]
        public Coordinates Coord { get; set; }
        
        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class Coordinates
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        
        [JsonProperty("lon")]
        public double Lon { get; set; }
    }

    public class WeatherInsight
    {
        public WeatherStatus Status { get; set; }
        public bool Alert { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public string Recommendation { get; set; }
        
        // Detailed weather data
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public string WeatherDescription { get; set; }
        public string WeatherIcon { get; set; }
        public double WindSpeed { get; set; }
        public double ProbabilityOfRain { get; set; }
        public DateTime Timestamp { get; set; }
        
        public string IconUrl => $"https://openweathermap.org/img/wn/{WeatherIcon}@2x.png";
    }

    public enum WeatherStatus
    {
        Unknown,
        Good,      // Clear/Clouds - Perfect
        Neutral,   // Mixed conditions
        Rainy,     // Rain/Drizzle
        Stormy,    // Thunderstorm
        Hot,       // > 33°C
        Cool       // < 20°C
    }
}
