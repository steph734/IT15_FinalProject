using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;
using RealEstate.Services;
using RealEstate; // Add for db context
using RealEstate.Helpers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.Controllers;

public class PropertiesController : Controller
{
    private readonly PropertyCatalog _catalog;
    private readonly SubscriptionService _subs;
    private readonly ApplicationDBContext _db;
    private readonly PayMongoService _payMongo;
    private readonly IConfiguration _configuration;
    private readonly WeatherForecastService _weatherService;
    private readonly GeocodingService _geocodingService;

    public PropertiesController(PropertyCatalog catalog, SubscriptionService subs, ApplicationDBContext db, PayMongoService payMongo, IConfiguration configuration, WeatherForecastService weatherService, GeocodingService geocodingService)
    {
        _catalog = catalog;
        _subs = subs;
        _db = db;
        _payMongo = payMongo;
        _configuration = configuration;
        _weatherService = weatherService;
        _geocodingService = geocodingService;
    }

    public IActionResult Index(string? location, string? priceRange, decimal? maxPrice, int page = 1, int pageSize = 5)
    {
        priceRange = string.IsNullOrWhiteSpace(priceRange) ? "any" : priceRange;

        var query = _db.Properties.AsQueryable();

        // Apply location filter
        if (!string.IsNullOrWhiteSpace(location) && location != "Any")
        {
            query = query.Where(p => p.Location.Contains(location));
        }

        // Apply price range filter
        if (!string.IsNullOrWhiteSpace(priceRange) && priceRange != "any")
        {
            switch (priceRange.ToLower())
            {
                case "p1":
                    maxPrice = maxPrice ?? 8000000;
                    query = query.Where(p => p.BasePrice <= maxPrice);
                    break;
                case "p2":
                    query = query.Where(p => p.BasePrice >= 8000000 && p.BasePrice <= 15000000);
                    break;
                case "p3":
                    query = query.Where(p => p.BasePrice >= 15000000 && p.BasePrice <= 30000000);
                    break;
                case "p4":
                    query = query.Where(p => p.BasePrice >= 30000000);
                    break;
            }
        }

        // Apply max price filter
        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.BasePrice <= maxPrice.Value);
        }

        var total = query.Count();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var vm = new PropertiesIndexViewModel
        {
            Location = location,
            PriceRange = priceRange,
            MaxPrice = maxPrice,
            Properties = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
        return View(vm);
    }

    public IActionResult Grid(string? location, string? priceRange, decimal? maxPrice, int page = 1, int pageSize = 12)
    {
        priceRange = string.IsNullOrWhiteSpace(priceRange) ? "any" : priceRange;

        var query = _db.Properties.AsQueryable();

        // Apply location filter
        if (!string.IsNullOrWhiteSpace(location) && location != "Any")
        {
            query = query.Where(p => p.Location.Contains(location));
        }

        // Apply price range filter
        if (!string.IsNullOrWhiteSpace(priceRange) && priceRange != "any")
        {
            switch (priceRange.ToLower())
            {
                case "p1":
                    maxPrice = maxPrice ?? 8000000;
                    query = query.Where(p => p.BasePrice <= maxPrice);
                    break;
                case "p2":
                    query = query.Where(p => p.BasePrice >= 8000000 && p.BasePrice <= 15000000);
                    break;
                case "p3":
                    query = query.Where(p => p.BasePrice >= 15000000 && p.BasePrice <= 30000000);
                    break;
                case "p4":
                    query = query.Where(p => p.BasePrice >= 30000000);
                    break;
            }
        }

        // Apply max price filter
        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.BasePrice <= maxPrice.Value);
        }

        var total = query.Count();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var paged = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var vm = new PropertiesIndexViewModel
        {
            Location = location,
            PriceRange = priceRange,
            MaxPrice = maxPrice,
            Properties = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
        return View(vm);
    }

    public IActionResult Details(int id)
    {
        var property = _db.Properties.Find(id);
        if (property is null)
            return NotFound();

        // Check if user is broker, redirect to broker dashboard view
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole == "Broker" || userRole == "Agent")
        {
            return RedirectToAction("ApprovedPropertyDetail", "Broker", new { id = id });
        }

        var vm = new PropertyDetailsViewModel
        {
            Property = property,
            Agent = null, // AgentId removed from Property model
            Schedule = new ScheduleViewingViewModel { PropertyId = property.PropertyId }
        };
        return View(vm);
    }

    public IActionResult RequestViewing(int id)
    {
        var property = _db.Properties.Find(id);
        if (property is null)
            return NotFound();

        var vm = new PropertyDetailsViewModel
        {
            Property = property,
            Agent = null, // AgentId removed from Property model
            Schedule = new ScheduleViewingViewModel { PropertyId = property.PropertyId }
        };
        return View(vm);
    }
    [HttpGet]
    public IActionResult SendInquiry(int propertyId)
    {
        var property = _db.Properties.Find(propertyId);
        if (property is null)
            return NotFound();

        var vm = new AgentInquiryViewModel
        {
            PropertyId = propertyId,
            AgentId = property.AgentId
        };
        ViewBag.PropertyTitle = property.Title;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SendInquiry(AgentInquiryViewModel model)
    {
        var property = _db.Properties.Find(model.PropertyId);
        var agent = model.AgentId.HasValue ? _catalog.GetAgent(model.AgentId.Value) : null;
        if (property == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var vm = new PropertyDetailsViewModel
            {
                Property = property,
                Agent = agent,
                Inquiry = model
            };
            return View("Details", vm);
        }

        // TODO: Send email or store inquiry (stub)
        TempData["InquirySuccess"] = "Your message has been sent to the agent.";
        return RedirectToAction("Details", new { id = model.PropertyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ScheduleViewing([Bind(Prefix = "Schedule")] ScheduleViewingViewModel model)
    {
        var property = _db.Properties.Find(model.PropertyId);
        if (property is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            return View("Details", new PropertyDetailsViewModel
            {
                Property = property,
                Agent = null, // AgentId removed from Property model
                Schedule = model
            });
        }

        // Save customer to DB and link to viewing
        var customer = new Customer
        {
            FullName = model.Name,
            Email = model.Email,
            Phone = model.Phone
        };
        _db.Customers.Add(customer);
        _db.SaveChanges();
        model.CustomerId = customer.CustomerId;

        // Save viewing appointment with all details including photo URL
        var viewing = new ViewingAppointment
        {
            PropertyId = model.PropertyId,
            CustomerId = customer.CustomerId,
            CustomerName = model.Name,
            CustomerEmail = model.Email,
            CustomerPhone = model.Phone,
            CustomerPhotoUrl = model.CustomerPhotoUrl,
            WhenUtc = model.PreferredDate,
            PreferredTime = model.PreferredTime ?? string.Empty,
            NumberOfVisitors = model.NumberOfVisitors,
            BuyerType = model.BuyerType ?? string.Empty,
            FinancingStatus = model.FinancingStatus ?? string.Empty,
            InformationSource = model.InformationSource ?? string.Empty,
            Notes = model.Notes ?? string.Empty,
            Status = AppointmentStatus.Scheduled,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.ViewingAppointments.Add(viewing);
        _db.SaveChanges();

        // Create Schedule record for the viewing date/time
        var startTime = TimeSpan.Parse(model.PreferredTime ?? "09:00");
        var schedule = new Schedule
        {
            UserId = customer.CustomerId,
            EmployeeId = property.AgentId ?? 0, // Use the property agent as the assigned employee (default to 0 if null)
            Day = model.PreferredDate,
            StartTime = startTime,
            EndTime = startTime.Add(TimeSpan.FromHours(1)) // 1-hour viewing appointment
        };
        _db.Schedules.Add(schedule);
        _db.SaveChanges();

        // Create Appointment record linking to the Schedule with all viewing details
        var appointment = new Appointment
        {
            PropertyId = model.PropertyId,
            AgentId = property.AgentId, // Use property's assigned agent
            ScheduleId = schedule.ScheduleId,
            Status = "Scheduled",
            // Customer Information
            CustomerName = model.Name,
            CustomerEmail = model.Email,
            CustomerPhone = model.Phone,
            CustomerPhotoUrl = model.CustomerPhotoUrl,
            // Viewing Details
            NumberOfVisitors = model.NumberOfVisitors,
            BuyerType = model.BuyerType,
            FinancingStatus = model.FinancingStatus,
            InformationSource = model.InformationSource,
            Notes = model.Notes,
            // Schedule Information
            PreferredDate = model.PreferredDate,
            PreferredTime = model.PreferredTime,
            // Appointment Type
            AppointmentType = model.AppointmentType ?? "self",
            // Meeting Point based on appointment type
            MeetingPoint = model.AppointmentType == "pickup" && !string.IsNullOrEmpty(model.CustomerAddress)
                ? $"Pickup: {model.CustomerAddress}"
                : property.Location ?? "Property Address",
            CreatedAt = DateTime.UtcNow
        };
        _db.Appointments.Add(appointment);
        _db.SaveChanges();

        HttpContext.Session.SetString(TransactionSessionKeys.PropertyId, model.PropertyId.ToString());
        HttpContext.Session.SetString(TransactionSessionKeys.CustomerName, model.Name);
        HttpContext.Session.SetString(TransactionSessionKeys.CustomerEmail, model.Email);
        HttpContext.Session.SetString(TransactionSessionKeys.CustomerPhone, model.Phone ?? string.Empty);

        TempData["ViewingSuccess"] = "Your viewing request has been submitted successfully!";
        return RedirectToAction("Details", new { id = model.PropertyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCheckout([Bind(Prefix = "Schedule")] ScheduleViewingViewModel model)
    {
        var property = _db.Properties.Find(model.PropertyId);
        if (property is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            return View("RequestViewing", new PropertyDetailsViewModel
            {
                Property = property,
                Agent = null, // AgentId removed from Property model
                Schedule = model
            });
        }

        // Store form data in session for retrieval after PayMongo redirect
        HttpContext.Session.SetString("ViewingFormData", JsonSerializer.Serialize(model));

        // Create PayMongo checkout session
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var checkoutRequest = new PayMongoCheckoutRequest
        {
            PropertyId = model.PropertyId,
            CustomerName = model.Name,
            CustomerEmail = model.Email,
            Amount = 500, // Booking fee: 500 PHP
            Currency = "PHP",
            Description = $"Property Viewing Booking Fee - {property.Title}",
            SuccessUrl = $"{baseUrl}/Properties/PaymentSuccess",
            CancelUrl = $"{baseUrl}/Properties/PaymentCancel?propertyId={model.PropertyId}"
        };

        var checkoutResponse = await _payMongo.CreateCheckoutSessionAsync(checkoutRequest);

        if (checkoutResponse.Success)
        {
            // Store checkout session ID for verification
            HttpContext.Session.SetString("PayMongoCheckoutSessionId", checkoutResponse.CheckoutSessionId);
            return Redirect(checkoutResponse.CheckoutUrl);
        }

        TempData["ErrorMessage"] = $"Payment initialization failed: {checkoutResponse.ErrorMessage}";
        return RedirectToAction("RequestViewing", new { id = model.PropertyId });
    }

    [HttpGet]
    public IActionResult PaymentSuccess()
    {
        // Retrieve form data from session
        var formDataJson = HttpContext.Session.GetString("ViewingFormData");
        if (string.IsNullOrEmpty(formDataJson))
        {
            TempData["ErrorMessage"] = "Session expired. Please submit your viewing request again.";
            return RedirectToAction("Index");
        }

        var model = JsonSerializer.Deserialize<ScheduleViewingViewModel>(formDataJson);
        if (model == null || string.IsNullOrWhiteSpace(model.Name))
        {
            TempData["ErrorMessage"] = "Invalid session data. Please submit your viewing request again.";
            return RedirectToAction("Index");
        }

        var property = _db.Properties.Find(model.PropertyId);
        if (property is null)
            return NotFound();

        // Save customer to DB
        var customer = new Customer
        {
            FullName = model.Name,
            Email = model.Email,
            Phone = model.Phone
        };
        _db.Customers.Add(customer);
        _db.SaveChanges();

        // Save viewing appointment with all details
        var viewing = new ViewingAppointment
        {
            PropertyId = model.PropertyId,
            CustomerId = customer.CustomerId,
            CustomerName = model.Name,
            CustomerEmail = model.Email,
            CustomerPhone = model.Phone,
            WhenUtc = model.PreferredDate,
            PreferredTime = model.PreferredTime,
            NumberOfVisitors = model.NumberOfVisitors,
            BuyerType = model.BuyerType,
            FinancingStatus = model.FinancingStatus,
            InformationSource = model.InformationSource,
            Notes = model.Notes,
            Status = AppointmentStatus.Scheduled,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.ViewingAppointments.Add(viewing);
        _db.SaveChanges();

        // Clear session data
        HttpContext.Session.Remove("ViewingFormData");
        HttpContext.Session.Remove("PayMongoCheckoutSessionId");

        // Set success flags
        TempData["ViewingSuccess"] = "Your viewing request has been submitted and payment received!";
        TempData["PaymentSuccess"] = true;

        return RedirectToAction("RequestViewing", new { id = model.PropertyId });
    }

    [HttpGet]
    public IActionResult PaymentCancel(int propertyId)
    {
        // Clear session data
        HttpContext.Session.Remove("ViewingFormData");
        HttpContext.Session.Remove("PayMongoCheckoutSessionId");

        TempData["WarningMessage"] = "Payment was cancelled. Your viewing request was not submitted. You can try again.";
        return RedirectToAction("RequestViewing", new { id = propertyId });
    }


    // ── Buy Property ────────────────────────────────────────────────────────────
    // GET handler to prevent 404 errors when users access URL directly
    [HttpGet]
    [Route("Properties/BuyProperty")]
    public IActionResult BuyProperty_Get()
    {
        TempData["InfoMessage"] = "Please use the 'Buy Property' button on a property details page.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Properties/BuyProperty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuyProperty(
        [FromForm] int propertyId,
        [FromForm] string fullName,
        [FromForm] string email,
        [FromForm] string phoneNumber,
        [FromForm] string? address,
        [FromForm] decimal totalAmount,
        [FromForm] string paymentMethod)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(fullName))
            {
                TempData["ErrorMessage"] = "Full name is required.";
                return RedirectToAction("Details", new { id = propertyId });
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "Email address is required.";
                return RedirectToAction("Details", new { id = propertyId });
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                TempData["ErrorMessage"] = "Phone number is required.";
                return RedirectToAction("Details", new { id = propertyId });
            }

            if (totalAmount <= 0)
            {
                TempData["ErrorMessage"] = "Invalid purchase amount.";
                return RedirectToAction("Details", new { id = propertyId });
            }

            var property = _db.Properties.Find(propertyId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "Property not found.";
                return RedirectToAction("Details", new { id = propertyId });
            }

            // Create transaction record with complete database mapping
            var transaction = new Transaction
            {
                // Primary Key: TransactionId (Auto-generated)
                PropertyId = propertyId,                          // FK → Properties table
                // AgentId = null,                               // FK → Users table (nullable, assign later)
                // CustomerId = null,                            // FK → Users table (nullable, link if user exists)
                
                // Customer Information (from form)
                CustomerName = fullName.Trim(),                 // ← Full Name field
                CustomerEmail = email.Trim(),                   // ← Email Address field
                CustomerPhone = phoneNumber.Trim(),             // ← Phone Number field
                
                // Financial Information
                SellingPrice = totalAmount,                     // ← Total Amount (calculated with fees)
                
                // Status Tracking
                Status = "Pending",                             // Initial status (Pending/Closed/Cancelled)
                CreatedAt = DateTime.UtcNow,                    // Auto-generated timestamp
                // ClosedAt = null,                            // Set when transaction closes
                
                // Additional Notes
                Notes = $"Payment Method: {paymentMethod?.Trim()}\nBuyer Address: {address?.Trim()}\nProperty: {property.Title}\nLocation: {property.Location}"
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            // Log success
            Console.WriteLine($"[SUCCESS] Transaction created: ID={transaction.TransactionId}, Property={propertyId}, Customer={fullName}, Amount={totalAmount}, Payment={paymentMethod}");

            TempData["SuccessMessage"] = $"Your purchase request has been submitted! Total: ₱{totalAmount:N0}. We will contact you shortly.";
            TempData["TransactionId"] = transaction.TransactionId;
            
            return RedirectToAction("Details", new { id = propertyId });
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"[ERROR] Purchase failed: {ex.Message}\n{ex.StackTrace}");
            TempData["ErrorMessage"] = $"Error processing purchase: {ex.Message}";
            return RedirectToAction("Details", new { id = propertyId });
        }
    }

    // ==================== WEATHER FORECAST API ====================

    /// <summary>
    /// Gets weather forecast for a property's appointment date
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetWeatherForecast(int propertyId, DateTime appointmentDate)
    {
        try
        {
            var property = await _db.Properties.FindAsync(propertyId);
            if (property == null)
                return NotFound(new { error = "Property not found" });

            if (!property.Latitude.HasValue || !property.Longitude.HasValue)
                return BadRequest(new { error = "Property coordinates not available" });

            // Get forecast for the appointment date/time
            var forecast = await _weatherService.GetForecastForDateTimeAsync(
                property.Latitude.Value, 
                property.Longitude.Value, 
                appointmentDate);

            if (forecast == null)
                return NotFound(new { error = "Weather data not available for this date" });

            // Analyze and return insights
            var insight = _weatherService.AnalyzeWeatherForAppointment(forecast);

            return Json(new
            {
                success = true,
                weather = new
                {
                    temperature = insight.Temperature,
                    feelsLike = insight.FeelsLike,
                    humidity = insight.Humidity,
                    description = insight.WeatherDescription,
                    icon = insight.IconUrl,
                    windSpeed = insight.WindSpeed,
                    rainProbability = insight.ProbabilityOfRain,
                    timestamp = insight.Timestamp
                },
                insight = new
                {
                    status = insight.Status.ToString(),
                    alert = insight.Alert,
                    alertType = insight.AlertType,
                    message = insight.Message,
                    recommendation = insight.Recommendation
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Weather forecast failed: {ex.Message}");
            return StatusCode(500, new { error = "Failed to fetch weather forecast", details = ex.Message });
        }
    }

    /// <summary>
    /// Gets full day forecast for appointment planning
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDayWeatherForecast(int propertyId, DateTime date)
    {
        try
        {
            var property = await _db.Properties.FindAsync(propertyId);
            if (property == null)
                return NotFound(new { error = "Property not found" });

            if (!property.Latitude.HasValue || !property.Longitude.HasValue)
                return BadRequest(new { error = "Property coordinates not available" });

            var forecasts = await _weatherService.GetDayForecastAsync(
                property.Latitude.Value, 
                property.Longitude.Value, 
                date);

            var weatherData = forecasts.Select(f => {
                var insight = _weatherService.AnalyzeWeatherForAppointment(f);
                return new
                {
                    time = f.DateTime.ToString("h:mm tt"),
                    temperature = insight.Temperature,
                    description = insight.WeatherDescription,
                    icon = insight.IconUrl,
                    rainProbability = insight.ProbabilityOfRain,
                    status = insight.Status.ToString(),
                    alert = insight.Alert
                };
            });

            return Json(new
            {
                success = true,
                date = date.ToString("yyyy-MM-dd"),
                location = property.Location,
                forecasts = weatherData
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Day weather forecast failed: {ex.Message}");
            return StatusCode(500, new { error = "Failed to fetch day forecast", details = ex.Message });
        }
    }

    // ==================== SELF-VISIT APPOINTMENT ====================

    /// <summary>
    /// Store self-visit appointment data
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> StoreSelfVisit([FromBody] SelfVisitRequest request)
    {
        try
        {
            var property = await _db.Properties.FindAsync(request.PropertyId);
            if (property == null)
                return NotFound(new { error = "Property not found" });

            Console.WriteLine($"[DEBUG] Property ID: {property.PropertyId}, AgentId: {property.AgentId}, EmployeeId: {property.EmployeeId}");

            // Get current user ID to use as fallback
            var currentUserId = AuthorizationHelper.GetUserId(HttpContext);
            Console.WriteLine($"[DEBUG] Current User ID: {currentUserId}");

            // Create appointment directly without schedule for self-visit
            var appointment = new Appointment
            {
                PropertyId = request.PropertyId,
                AgentId = property.AgentId ?? currentUserId, // Use property's assigned agent or current user
                EmployeeId = property.EmployeeId ?? currentUserId, // Use property's employee or current user
                ScheduleId = null, // No schedule for self-visit
                Status = "Scheduled",
                CustomerName = request.FullName,
                CustomerEmail = request.Email,
                CustomerPhone = request.Phone,
                AppointmentType = "self",
                PreferredDate = DateTime.TryParse(request.PreferredDate, out var preferredDate) ? preferredDate : DateTime.Today,
                PreferredTime = request.PreferredTime ?? "Self-Visit",
                IdType = request.IdType,
                IdNumber = request.IdNumber,
                TransportationMethod = request.Transportation,
                CreatedAt = DateTime.UtcNow
            };
            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            // Create notification for broker/agent
            var notificationAgentId = property.AgentId ?? currentUserId;
            var notificationEmployeeId = property.EmployeeId ?? currentUserId;

            Console.WriteLine($"[DEBUG] Creating notification with AgentId: {notificationAgentId}, EmployeeId: {notificationEmployeeId}");

            var notification = new Notification
            {
                AgentId = notificationAgentId, // Use AgentId or current user
                EmployeeId = notificationEmployeeId, // Use EmployeeId or current user
                AppointmentId = appointment.AppointmentId,
                Title = "New Self-Visit Appointment",
                Message = $"A new self-visit appointment has been scheduled for property '{property.Title}' by {request.FullName}. Customer email: {request.Email}, Phone: {request.Phone}.",
                NotificationType = "SelfVisit",
                RelatedEntityId = appointment.AppointmentId,
                RelatedEntityType = "Appointment",
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow
            };
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                appointmentId = appointment.AppointmentId,
                message = "Self-visit appointment recorded successfully"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Self-visit storage failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException?.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = "Failed to store self-visit data", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Store agent pickup appointment data
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> StorePickupAppointment([FromBody] SelfVisitRequest request)
    {
        try
        {
            var property = await _db.Properties.FindAsync(request.PropertyId);
            if (property == null)
                return NotFound(new { error = "Property not found" });

            // Get current user ID to use as fallback
            var currentUserId = AuthorizationHelper.GetUserId(HttpContext);
            Console.WriteLine($"[DEBUG] Property ID: {property.PropertyId}, AgentId: {property.AgentId}, EmployeeId: {property.EmployeeId}");
            Console.WriteLine($"[DEBUG] Current User ID: {currentUserId}");

            // Create appointment for agent pickup
            var appointment = new Appointment
            {
                PropertyId = request.PropertyId,
                AgentId = property.AgentId ?? currentUserId,
                EmployeeId = property.EmployeeId ?? currentUserId,
                ScheduleId = null,
                Status = "Scheduled",
                CustomerName = request.FullName,
                CustomerEmail = request.Email,
                CustomerPhone = request.Phone,
                AppointmentType = "pickup",
                PreferredDate = DateTime.TryParse(request.PreferredDate, out var preferredDate) ? preferredDate : DateTime.Today,
                PreferredTime = request.PreferredTime ?? "Pickup",
                CustomerAddress = request.CustomerAddress,
                CustomerLatitude = request.CustomerLatitude,
                CustomerLongitude = request.CustomerLongitude,
                CreatedAt = DateTime.UtcNow
            };
            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            // Create notification for broker/agent
            var notificationAgentId = property.AgentId ?? currentUserId;
            var notificationEmployeeId = property.EmployeeId ?? currentUserId;

            Console.WriteLine($"[DEBUG] Creating notification with AgentId: {notificationAgentId}, EmployeeId: {notificationEmployeeId}");

            var notification = new Notification
            {
                AgentId = notificationAgentId,
                EmployeeId = notificationEmployeeId,
                AppointmentId = appointment.AppointmentId,
                Title = "New Pickup Appointment",
                Message = $"A new pickup appointment has been scheduled for property '{property.Title}' by {request.FullName}. Customer email: {request.Email}, Phone: {request.Phone}. Pickup location: {request.CustomerAddress}.",
                NotificationType = "Pickup",
                RelatedEntityId = appointment.AppointmentId,
                RelatedEntityType = "Appointment",
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow
            };
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                appointmentId = appointment.AppointmentId,
                message = "Pickup appointment recorded successfully"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Pickup appointment storage failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException?.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = "Failed to store pickup appointment data", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    // ==================== GEOCODING API (LocationIQ) ====================

    /// <summary>
    /// Geocode an address to get Latitude/Longitude coordinates
    /// Used when seller types an address to get map coordinates
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GeocodeAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return BadRequest(new { error = "Address is required" });

        try
        {
            var result = await _geocodingService.GeocodeAddressAsync(address);
            
            if (result == null)
                return NotFound(new { error = "Address not found. Please try a more specific address." });

            return Json(new
            {
                success = true,
                latitude = result.Latitude,
                longitude = result.Longitude,
                displayName = result.DisplayName,
                confidence = result.Confidence,
                fullAddress = result.Address
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Geocoding failed: {ex.Message}");
            return StatusCode(500, new { error = "Geocoding service error", details = ex.Message });
        }
    }

    /// <summary>
    /// Reverse geocode - get address from coordinates
    /// Used when seller drags the map pin
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ReverseGeocode(double latitude, double longitude)
    {
        try
        {
            var address = await _geocodingService.ReverseGeocodeAsync(latitude, longitude);
            
            return Json(new
            {
                success = true,
                address = address,
                latitude = latitude,
                longitude = longitude
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Reverse geocoding failed: {ex.Message}");
            return StatusCode(500, new { error = "Reverse geocoding error", details = ex.Message });
        }
    }

    /// <summary>
    /// Validate if an address can be geocoded
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return BadRequest(new { error = "Address is required" });

        try
        {
            var isValid = await _geocodingService.IsValidAddressAsync(address);
            
            return Json(new
            {
                success = true,
                isValid = isValid,
                message = isValid ? "Address is valid" : "Address could not be verified"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Address validation failed: {ex.Message}");
            return StatusCode(500, new { error = "Validation error", details = ex.Message });
        }
    }

    /// <summary>
    /// Buy Property - Creates customer record and payment record
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> BuyProperty([FromBody] BuyPropertyRequest request)
    {
        try
        {
            // Validate property exists
            var property = await _db.Properties.FindAsync(request.PropertyId);
            if (property == null)
                return Json(new { success = false, error = "Property not found" });

            // Check if property is already sold/rented
            if (property.IsSold)
                return Json(new { success = false, error = "Property is no longer available" });

            // Validate property type matches request
            if (property.ListingType.ToString() != request.PropertyType)
                return Json(new { success = false, error = "Property type mismatch" });

            // Create or update customer
            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.PhoneNumber,
                    Address = request.Address,
                    Status = request.PropertyType == "Rent" ? "Leased" : "Purchased",
                    PaymentMethod = request.PaymentMethod,
                    PurchasedPropertyId = request.PropertyId,
                    CreatedDate = DateTime.UtcNow
                };
                _db.Customers.Add(customer);
            }
            else
            {
                customer.FullName = request.FullName;
                customer.Phone = request.PhoneNumber;
                customer.Address = request.Address;
                customer.Status = request.PropertyType == "Rent" ? "Leased" : "Purchased";
                customer.PaymentMethod = request.PaymentMethod;
                customer.PurchasedPropertyId = request.PropertyId;
                customer.LastContactedDate = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            // Create payment record
            var payment = new Payment
            {
                CustomerId = customer.CustomerId,
                PropertyId = request.PropertyId,
                PropertyType = request.PropertyType,
                PaymentFrequency = request.PaymentFrequency,
                Amount = request.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                ReferenceNumber = GenerateReferenceNumber(),
                Status = "Pending",
                CustomerName = request.FullName,
                CustomerEmail = request.Email,
                CustomerPhone = request.PhoneNumber,
                CustomerAddress = request.Address,
                CreatedAt = DateTime.UtcNow
            };
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            // Mark property as sold/rented
            property.IsSold = true;
            property.SoldDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            string actionType = request.PropertyType == "Rent" ? "Lease" : "Purchase";
            return Json(new
            {
                success = true,
                customerId = customer.CustomerId,
                paymentId = payment.PaymentId,
                referenceNumber = payment.ReferenceNumber,
                message = $"{actionType} completed successfully",
                propertyType = request.PropertyType
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Buy property failed: {ex.Message}");
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    private string GenerateReferenceNumber()
    {
        return $"PROP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
    }
}
