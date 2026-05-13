using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public class PayMongoService
    {
        private readonly string _secretKey;
        private readonly string _publicKey;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.paymongo.com/v1";

        public PayMongoService(string secretKey, string publicKey)
        {
            _secretKey = secretKey;
            _publicKey = publicKey;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Create a payment source (card token) using PayMongo
        /// </summary>
        public async Task<PayMongoSourceResponse> CreatePaymentSourceAsync(PayMongoSourceRequest request)
        {
            try
            {
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            type = "card",
                            amount = (int)(request.Amount * 100), // Convert to centavos
                            currency = request.Currency,
                            details = new
                            {
                                card_number = request.CardNumber,
                                exp_month = request.ExpMonth,
                                exp_year = request.ExpYear,
                                cvc = request.CVV,
                                billing = new
                                {
                                    name = request.CardholderName
                                }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request_message = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/sources")
                {
                    Content = content
                };

                request_message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"))}");

                var response = await _httpClient.SendAsync(request_message);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    return new PayMongoSourceResponse
                    {
                        Success = true,
                        SourceId = result.GetProperty("data").GetProperty("id").GetString(),
                        Status = result.GetProperty("data").GetProperty("attributes").GetProperty("status").GetString(),
                        Amount = request.Amount,
                        Currency = request.Currency
                    };
                }

                return new PayMongoSourceResponse
                {
                    Success = false,
                    ErrorMessage = responseContent
                };
            }
            catch (Exception ex)
            {
                return new PayMongoSourceResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Create a payment intent using PayMongo
        /// </summary>
        public async Task<PayMongoPaymentResponse> CreatePaymentIntentAsync(PayMongoPaymentRequest request)
        {
            try
            {
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            amount = (int)(request.Amount * 100), // Convert to centavos
                            currency = request.Currency,
                            payment_method_allowed = new[] { "card" },
                            payment_method_options = new
                            {
                                card = new
                                {
                                    request_three_d_secure = "automatic"
                                }
                            },
                            description = request.Description,
                            statement_descriptor = "EstateFlow Payment",
                            metadata = new
                            {
                                customer_id = request.CustomerId.ToString(),
                                order_id = request.OrderId
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request_message = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/payment_intents")
                {
                    Content = content
                };

                request_message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"))}");

                var response = await _httpClient.SendAsync(request_message);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var data = result.GetProperty("data");
                    return new PayMongoPaymentResponse
                    {
                        Success = true,
                        PaymentIntentId = data.GetProperty("id").GetString(),
                        Status = data.GetProperty("attributes").GetProperty("status").GetString(),
                        ClientKey = data.GetProperty("attributes").GetProperty("client_key").GetString(),
                        Amount = request.Amount,
                        Currency = request.Currency
                    };
                }

                return new PayMongoPaymentResponse
                {
                    Success = false,
                    ErrorMessage = responseContent
                };
            }
            catch (Exception ex)
            {
                return new PayMongoPaymentResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Attach a payment method to a payment intent
        /// </summary>
        public async Task<PayMongoAttachResponse> AttachPaymentMethodAsync(string paymentIntentId, string paymentMethodId)
        {
            try
            {
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            payment_method = paymentMethodId
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request_message = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/payment_intents/{paymentIntentId}/attach")
                {
                    Content = content
                };

                request_message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"))}");

                var response = await _httpClient.SendAsync(request_message);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var data = result.GetProperty("data");
                    return new PayMongoAttachResponse
                    {
                        Success = true,
                        Status = data.GetProperty("attributes").GetProperty("status").GetString()
                    };
                }

                return new PayMongoAttachResponse
                {
                    Success = false,
                    ErrorMessage = responseContent
                };
            }
            catch (Exception ex)
            {
                return new PayMongoAttachResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Create a PayMongo checkout session for hosted payment page (GCash, Maya, Card)
        /// </summary>
        public async Task<PayMongoCheckoutResponse> CreateCheckoutSessionAsync(PayMongoCheckoutRequest request)
        {
            try
            {
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            amount = (int)(request.Amount * 100),
                            currency = request.Currency,
                            description = request.Description,
                            statement_descriptor = "EstateFlow",
                            metadata = new
                            {
                                customer_name = request.CustomerName,
                                customer_email = request.CustomerEmail,
                                property_id = request.PropertyId.ToString()
                            },
                            success_url = request.SuccessUrl,
                            cancel_url = request.CancelUrl,
                            payment_method_types = new[] { "gcash", "paymaya", "card" }
                        }
                    }
                };
        
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
        
                var request_message = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/checkout_sessions")
                {
                    Content = content
                };
        
                request_message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"))}");
        
                var response = await _httpClient.SendAsync(request_message);
                var responseContent = await response.Content.ReadAsStringAsync();
        
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var data = result.GetProperty("data");
                    var attributes = data.GetProperty("attributes");
                    return new PayMongoCheckoutResponse
                    {
                        Success = true,
                        CheckoutSessionId = data.GetProperty("id").GetString() ?? string.Empty,
                        CheckoutUrl = attributes.GetProperty("checkout_url").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency
                    };
                }
        
                return new PayMongoCheckoutResponse
                {
                    Success = false,
                    ErrorMessage = responseContent
                };
            }
            catch (Exception ex)
            {
                return new PayMongoCheckoutResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        
        /// <summary>
        /// Retrieve payment intent details
        /// </summary>
        public async Task<PayMongoPaymentResponse> RetrievePaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var request_message = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/payment_intents/{paymentIntentId}");
                request_message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"))}");

                var response = await _httpClient.SendAsync(request_message);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var data = result.GetProperty("data");
                    return new PayMongoPaymentResponse
                    {
                        Success = true,
                        PaymentIntentId = data.GetProperty("id").GetString() ?? string.Empty,
                        Status = data.GetProperty("attributes").GetProperty("status").GetString() ?? string.Empty,
                        Amount = Convert.ToDecimal(data.GetProperty("attributes").GetProperty("amount").GetDouble()) / 100
                    };
                }

                return new PayMongoPaymentResponse
                {
                    Success = false,
                    ErrorMessage = responseContent
                };
            }
            catch (Exception ex)
            {
                return new PayMongoPaymentResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    // Request/Response Models
    public class PayMongoSourceRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PHP";
        public string CardNumber { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string CVV { get; set; }
        public string CardholderName { get; set; }
    }

    public class PayMongoSourceResponse
    {
        public bool Success { get; set; }
        public string? SourceId { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PayMongoPaymentRequest
    {
        public int CustomerId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PHP";
        public string Description { get; set; }
    }

    public class PayMongoPaymentResponse
    {
        public bool Success { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? Status { get; set; }
        public string? ClientKey { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PayMongoAttachResponse
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PayMongoCheckoutRequest
    {
        public int PropertyId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PHP";
        public string Description { get; set; } = string.Empty;
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }

    public class PayMongoCheckoutResponse
    {
        public bool Success { get; set; }
        public string CheckoutSessionId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
