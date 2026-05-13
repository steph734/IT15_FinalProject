using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Attributes;
using RealEstate.Helpers;
using RealEstate.Services;
using RealEstate.Models;
using System.Text.Json;

namespace RealEstate.Controllers;

[Route("broker")]
[RequireRole("Broker", "Agent")]
public class BrokerController : Controller
{
    private readonly PropertyCatalog _catalog;
    private readonly ApplicationDBContext _context;
    private readonly IConfiguration _configuration;
    private readonly RentCastService _rentCastService;

    public BrokerController(PropertyCatalog catalog, ApplicationDBContext context, IConfiguration configuration, RentCastService rentCastService)
    {
        _catalog = catalog;
        _context = context;
        _configuration = configuration;
        _rentCastService = rentCastService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction("Dashboard");
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        return RedirectToAction("Analytics");
    }

    [HttpGet("dashboard/analytics")]
    public IActionResult Analytics()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("dashboard/my-dashboard")]
    public IActionResult MyDashboard()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("dashboard/customers")]
    public IActionResult Customers()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("dashboard/calendar")]
    public IActionResult Calendar()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("sales")]
    public IActionResult Sales()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";

        var recent = _context.CommissionDeals
            .Where(d => d.AgentUserId == brokerId.Value)
            .OrderByDescending(d => d.CreatedAtUtc)
            .Take(20)
            .ToList();

        return View("~/Views/Broker/SalesPipeline.cshtml", recent);
    }

    [HttpPost("sales/close")]
    public IActionResult CloseDeal([FromForm] string propertyLabel, [FromForm] decimal grossCommission)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        propertyLabel = (propertyLabel ?? string.Empty).Trim();
        if (propertyLabel.Length == 0)
            propertyLabel = "Closed Deal";

        if (grossCommission < 0)
            grossCommission = 0;

        var agentSplit = 70m;
        var companySplit = 30m;
        var agentPayout = Math.Round(grossCommission * (agentSplit / 100m), 2, MidpointRounding.AwayFromZero);

        var deal = new CommissionDeal
        {
            PropertyLabel = propertyLabel,
            AgentUserId = brokerId.Value,
            BrokerUserId = brokerId.Value,
            GrossCommission = grossCommission,
            AgentSplitPercent = agentSplit,
            CompanySplitPercent = companySplit,
            AddOnsTotal = 0m,
            AgentPayoutAmount = agentPayout,
            DealDocumentsJson = "[]",
            Status = CommissionDealStatus.PendingManagerApproval,
            AccountingPayoutStatus = AccountingPayoutStatus.AwaitingReview,
            CreatedAtUtc = DateTime.UtcNow,
            AuditJson = JsonSerializer.Serialize(new
            {
                createdBy = brokerId.Value,
                createdAtUtc = DateTime.UtcNow,
                split = new { agent = agentSplit, company = companySplit }
            })
        };

        _context.CommissionDeals.Add(deal);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Deal marked as Closed and sent to Manager for approval.";
        return RedirectToAction("Sales");
    }

    [HttpGet("customers")]
    public IActionResult CustomerList()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("customers/create")]
    public IActionResult CreateCustomer()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpPost("customers/store")]
    public async Task<IActionResult> StoreCustomer([FromForm] CreateCustomerDTO dto, [FromServices] PayMongoService payMongoService)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        try
        {
            // Create new customer
            var customer = new Customer
            {
                BrokerId = brokerId,
                FullName = $"{dto.FirstName} {dto.LastName}",
                Email = dto.Email,
                Phone = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Country = dto.Country,
                PropertyType = dto.PropertyType,
                InterestedProperties = dto.InterestedProperties,
                MinBudget = dto.MinBudget,
                MaxBudget = dto.MaxBudget,
                Status = dto.Status,
                PaymentMethod = dto.PaymentMethod,
                CardholderName = dto.CardholderName,
                CardNumber = dto.CardNumber,
                ExpiryDate = dto.ExpiryDate,
                CVV = dto.CVV,
                Notes = dto.Notes,
                CreatedDate = DateTime.UtcNow,
                LastContactedDate = DateTime.UtcNow,
                IsActive = true
            };

            // Add customer to database first
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Process payment with PayMongo if amount is set
            decimal paymentAmount = dto.MaxBudget ?? 0;

            if (paymentAmount > 0)
            {
                try
                {
                    // Create payment intent
                    var paymentRequest = new PayMongoPaymentRequest
                    {
                        CustomerId = customer.CustomerId,
                        OrderId = $"ORD-{customer.CustomerId}-{DateTime.UtcNow.Ticks}",
                        Amount = paymentAmount,
                        Currency = "PHP",
                        Description = $"Property Purchase - {dto.FirstName} {dto.LastName}"
                    };

                    var paymentResponse = await payMongoService.CreatePaymentIntentAsync(paymentRequest);

                    if (paymentResponse.Success)
                    {
                        // Create payment transaction record
                        var transaction = new PaymentTransaction
                        {
                            CustomerId = customer.CustomerId,
                            PayMongoPaymentIntentId = paymentResponse.PaymentIntentId,
                            Amount = paymentAmount,
                            Currency = paymentResponse.Currency,
                            Status = paymentResponse.Status,
                            PaymentMethod = dto.PaymentMethod,
                            Description = paymentRequest.Description,
                            CreatedDate = DateTime.UtcNow,
                            IsProcessed = false
                        };

                        _context.PaymentTransactions.Add(transaction);
                        _context.SaveChanges();

                        // Store payment intent ID in session for verification
                        HttpContext.Session.SetString($"PaymentIntent_{customer.CustomerId}", paymentResponse.PaymentIntentId);
                        HttpContext.Session.SetString($"ClientKey_{customer.CustomerId}", paymentResponse.ClientKey);

                        // Redirect to payment confirmation page
                        TempData["CustomerId"] = customer.CustomerId;
                        TempData["PaymentIntentId"] = paymentResponse.PaymentIntentId;
                        TempData["Amount"] = paymentAmount;
                        return RedirectToAction("PaymentConfirmation", new { customerId = customer.CustomerId });
                    }
                    else
                    {
                        // Payment creation failed, but customer was created
                        TempData["WarningMessage"] = $"Customer created but payment processing failed: {paymentResponse.ErrorMessage}";
                        return RedirectToAction("CustomerList");
                    }
                }
                catch (Exception paymentEx)
                {
                    // Payment processing error, customer still created
                    TempData["WarningMessage"] = $"Customer created but payment processing failed: {paymentEx.Message}";
                    return RedirectToAction("CustomerList");
                }
            }
            else
            {
                // No payment needed
                TempData["SuccessMessage"] = "Customer created successfully!";
                return RedirectToAction("CustomerList");
            }
        }
        catch (Exception ex)
        {
            // Log error and return with error message
            TempData["ErrorMessage"] = $"Error creating customer: {ex.Message}";
            return RedirectToAction("CreateCustomer");
        }
    }

    [HttpGet("payment-confirmation")]
    public IActionResult PaymentConfirmation(int customerId)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        if (customer == null)
            return NotFound();

        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue || customer.BrokerId != brokerId)
            return Unauthorized();

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        ViewBag.Customer = customer;
        ViewBag.PayMongoPublicKey = _configuration["PayMongo:PublicKey"];

        return View(customer);
    }

    [HttpPost("payment/process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentProcessRequest request, [FromServices] PayMongoService payMongoService)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return Json(new { success = false, message = "Unauthorized" });

        try
        {
            // Get customer
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == request.CustomerId && c.BrokerId == brokerId);
            if (customer == null)
                return Json(new { success = false, message = "Customer not found" });

            // Get payment transaction
            var transaction = _context.PaymentTransactions.FirstOrDefault(t => t.CustomerId == request.CustomerId);
            if (transaction == null)
                return Json(new { success = false, message = "Payment transaction not found" });

            try
            {
                // Retrieve payment intent to check status
                var paymentStatus = await payMongoService.RetrievePaymentIntentAsync(transaction.PayMongoPaymentIntentId);

                if (paymentStatus.Success)
                {
                    // Update transaction status
                    transaction.Status = paymentStatus.Status;
                    transaction.UpdatedDate = DateTime.UtcNow;

                    if (paymentStatus.Status == "succeeded")
                    {
                        transaction.IsProcessed = true;
                        customer.LastContactedDate = DateTime.UtcNow;
                        customer.IsActive = true;

                        _context.SaveChanges();

                        return Json(new 
                        { 
                            success = true, 
                            message = "Payment processed successfully!",
                            transactionId = transaction.Id,
                            status = transaction.Status
                        });
                    }
                    else if (paymentStatus.Status == "awaiting_payment_method")
                    {
                        _context.SaveChanges();
                        return Json(new 
                        { 
                            success = false, 
                            message = "Payment method awaiting confirmation. Please try again.",
                            status = transaction.Status
                        });
                    }
                    else if (paymentStatus.Status == "failed")
                    {
                        transaction.IsProcessed = false;
                        transaction.ErrorMessage = "Payment failed";
                        _context.SaveChanges();

                        return Json(new 
                        { 
                            success = false, 
                            message = "Payment failed. Please check your card details and try again.",
                            status = transaction.Status
                        });
                    }
                    else
                    {
                        _context.SaveChanges();
                        return Json(new 
                        { 
                            success = false, 
                            message = $"Payment status: {paymentStatus.Status}",
                            status = transaction.Status
                        });
                    }
                }
                else
                {
                    transaction.ErrorMessage = paymentStatus.ErrorMessage;
                    transaction.Status = "failed";
                    _context.SaveChanges();

                    return Json(new 
                    { 
                        success = false, 
                        message = "Unable to verify payment status. Please try again later."
                    });
                }
            }
            catch (Exception verifyEx)
            {
                transaction.ErrorMessage = verifyEx.Message;
                transaction.Status = "error";
                _context.SaveChanges();

                return Json(new 
                { 
                    success = false, 
                    message = "Error verifying payment. Please try again."
                });
            }
        }
        catch (Exception ex)
        {
            return Json(new 
            { 
                success = false, 
                message = $"Error processing payment: {ex.Message}"
            });
        }
    }

    [HttpPost("payment/webhook")]
    public async Task<IActionResult> PaymentWebhook([FromBody] PayMongoWebhookRequest webhookData, [FromServices] PayMongoService payMongoService)
    {
        try
        {
            if (webhookData?.Data?.Attributes?.Status == "succeeded")
            {
                // Find transaction by PayMongo payment intent ID
                var paymentIntentId = webhookData.Data.Id;
                var transaction = _context.PaymentTransactions
                    .FirstOrDefault(t => t.PayMongoPaymentIntentId == paymentIntentId);

                if (transaction != null)
                {
                    transaction.Status = "succeeded";
                    transaction.IsProcessed = true;
                    transaction.UpdatedDate = DateTime.UtcNow;
                    transaction.WebhookResponse = System.Text.Json.JsonSerializer.Serialize(webhookData);

                    var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == transaction.CustomerId);
                    if (customer != null)
                    {
                        customer.IsActive = true;
                        customer.LastContactedDate = DateTime.UtcNow;
                    }

                    _context.SaveChanges();
                }
            }
            else if (webhookData?.Data?.Attributes?.Status == "failed")
            {
                var paymentIntentId = webhookData.Data.Id;
                var transaction = _context.PaymentTransactions
                    .FirstOrDefault(t => t.PayMongoPaymentIntentId == paymentIntentId);

                if (transaction != null)
                {
                    transaction.Status = "failed";
                    transaction.IsProcessed = false;
                    transaction.UpdatedDate = DateTime.UtcNow;
                    transaction.WebhookResponse = System.Text.Json.JsonSerializer.Serialize(webhookData);
                    transaction.ErrorMessage = webhookData.Data?.Attributes?.ErrorMessage;

                    _context.SaveChanges();
                }
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the error but return OK to PayMongo
            return Ok(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("properties")]
    public IActionResult Properties(int page = 1, int pageSize = 10)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        var allProperties = _catalog.GetProperties().ToList();
        var total = allProperties.Count;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = allProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View(paged);
    }

    [HttpGet("property-list")]
    public IActionResult PropertyList(int page = 1, int pageSize = 10)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agentId = _context.Agents.Where(a => a.UserId == brokerId.Value).Select(a => a.Id).FirstOrDefault();
        var agentProperties = _context.Properties.Where(p => p.AgentId == agentId).ToList();

        var total = agentProperties.Count;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = agentProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View(paged);
    }

    [HttpGet("property-grid")]
    public IActionResult PropertyGrid(int page = 1, int pageSize = 12)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        var agentId = _context.Agents.Where(a => a.UserId == brokerId.Value).Select(a => a.Id).FirstOrDefault();
        var agentProperties = _context.Properties.Where(p => p.AgentId == agentId).ToList();

        var total = agentProperties.Count;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = agentProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View(paged);
    }

    [HttpGet("leads")]
    public IActionResult Leads()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("performance")]
    public IActionResult Performance()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("commissions")]
    public IActionResult Commissions()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("orders")]
    public IActionResult Orders(int page = 1, int pageSize = 10)
    {
        try
        {
            var brokerId = AuthorizationHelper.GetUserId(HttpContext);
            if (!brokerId.HasValue)
                return RedirectToAction("Login", "Admin");

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            // Sample/Seeded data instead of database
            var allOrders = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 1,
                    FullName = "David Nummi",
                    Email = "david@example.com",
                    Phone = "+231 06-75820711",
                    PropertyType = "Residential",
                    InterestedProperties = "123 Maple St, 456 Oak Ave",
                    MaxBudget = 2890123,
                    PaymentMethod = "card",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 2,
                    FullName = "Sinikka Penttinen",
                    Email = "sinikka@example.com",
                    Phone = "+231 47-23456789",
                    PropertyType = "Commercial",
                    InterestedProperties = "789 Pine Blvd",
                    MaxBudget = 2678901,
                    PaymentMethod = "cash",
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow.AddDays(-8),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 3,
                    FullName = "Jere Palmu",
                    Email = "jere@example.com",
                    Phone = "+231 73-34567890",
                    PropertyType = "Residential",
                    InterestedProperties = "101 Birch Ct, 202 Cedar Ln",
                    MaxBudget = 4123456,
                    PaymentMethod = "card",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddDays(-15),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 4,
                    FullName = "Ulla Nuorela",
                    Email = "ulla@example.com",
                    Phone = "+231 45-45678901",
                    PropertyType = "Residential",
                    InterestedProperties = "303 Elm St",
                    MaxBudget = 3456789,
                    PaymentMethod = "card",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 5,
                    FullName = "Tia Karppinen",
                    Email = "tia@example.com",
                    Phone = "+231 16-56789012",
                    PropertyType = "Industrial",
                    InterestedProperties = "404 Walnut Rd",
                    MaxBudget = 2789012,
                    PaymentMethod = "cash",
                    Status = "Inactive",
                    CreatedDate = DateTime.UtcNow.AddDays(-20),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 6,
                    FullName = "Harland R. Orsini",
                    Email = "harland@example.com",
                    Phone = "+231 82-67890123",
                    PropertyType = "Residential",
                    InterestedProperties = "505 Spruce St",
                    MaxBudget = 2456789,
                    PaymentMethod = "card",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddDays(-12),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 7,
                    FullName = "David Padgett",
                    Email = "david.p@example.com",
                    Phone = "+231 92-78901234",
                    PropertyType = "Commercial",
                    InterestedProperties = "606 Fir Ave",
                    MaxBudget = 1567890,
                    PaymentMethod = "card",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddDays(-3),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 8,
                    FullName = "Valerie Obrien",
                    Email = "valerie@example.com",
                    Phone = "+231 82-89012345",
                    PropertyType = "Residential",
                    InterestedProperties = "808 Willow Dr, 909 Aspen Ln",
                    MaxBudget = 1234567,
                    PaymentMethod = "card",
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow.AddDays(-7),
                    BrokerId = brokerId
                },
                new Customer
                {
                    CustomerId = 9,
                    FullName = "Adriana G. Faust",
                    Email = "adriana@example.com",
                    Phone = "+231 54-4775764",
                    PropertyType = "Apartment",
                    InterestedProperties = "1190 Barlow Street",
                    MaxBudget = 1502331,
                    PaymentMethod = "cash",
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow.AddDays(-2),
                    BrokerId = brokerId
                }
            };

            // Get total count
            var total = allOrders.Count;

            // Apply pagination
            var customers = allOrders
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.BrokerId = brokerId.Value;
            ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

            return View(customers);
        }
        catch (Exception ex)
        {
            return Content($"Error: {ex.Message}\n\nStack: {ex.StackTrace}");
        }
    }

    [HttpGet("orders-test")]
    public IActionResult OrdersTest()
    {
        try
        {
            var brokerId = AuthorizationHelper.GetUserId(HttpContext);
            if (!brokerId.HasValue)
                return Content("No broker ID");

            // Sample seeded data
            var count = 9; // Number of sample orders
            return Content($"Broker ID: {brokerId}, Sample order count: {count}");
        }
        catch (Exception ex)
        {
            return Content($"Error: {ex.Message}");
        }
    }

    [HttpGet("inbox")]
    public IActionResult Inbox(int page = 1, int pageSize = 10)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        // Fetch actual notifications from database for this broker
        var notifications = _context.Notifications
            .Where(n => n.EmployeeId == brokerId.Value)
            .Include(n => n.Appointment)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToList();

        // Convert notifications to inbox format
        var allMessages = notifications.Select(n => new
        {
            Id = n.NotificationId,
            From = n.Appointment?.CustomerName ?? "System",
            Email = n.Appointment?.CustomerEmail ?? "noreply@realestate.com",
            Subject = n.Title.Length > 30 ? n.Title.Substring(0, 30) + "..." : n.Title,
            FullSubject = n.Title,
            Message = n.Message,
            Date = n.CreatedAtUtc,
            Time = n.CreatedAtUtc.ToString("HH:mm"),
            Read = n.IsRead,
            Type = n.NotificationType.ToLower(),
            Avatar = GetInitials(n.Appointment?.CustomerName ?? "System"),
            AppointmentId = n.AppointmentId
        }).ToList<dynamic>();

        var total = allMessages.Count;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var messages = allMessages
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        ViewBag.UnreadCount = allMessages.Count(m => !m.Read);

        return View(messages);
    }

    private string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "??";
        
        var parts = name.Split(' ');
        if (parts.Length >= 2)
            return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
        return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
    }

    [HttpGet("messages")]
    public IActionResult Messages()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }

    // ══ Approved Listings (from Sellers) ════════════════════════════════════════

    [HttpGet("approved-listings")]
    public IActionResult ApprovedListings(string? type = null, string? search = null)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId   = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";

        var query = _context.Properties
            .AsNoTracking()
            .Where(p => p.IsApproved);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.PropertyType == type);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.Contains(search) || p.Location.Contains(search));

        var properties = query.OrderByDescending(p => p.DecisionAt).ToList();
        ViewBag.TypeFilter   = type;
        ViewBag.SearchFilter = search;
        return View("~/Views/Broker/ApprovedProperties.cshtml", properties);
    }

    [HttpGet("approved-listings/{id:int}")]
    [HttpGet("ApprovedPropertyDetail/{id:int}")]
    public async Task<IActionResult> ApprovedListingDetail(int id)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerId   = brokerId.Value;
        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";

        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PropertyId == id && p.IsApproved);

        if (property == null) return NotFound();

        return View("~/Views/Broker/ApprovedPropertyDetail.cshtml", property);
    }

    /// <summary>
    /// Refresh RentCast market data for a property - Broker Superuser Control
    /// </summary>
    [HttpPost("properties/{id:int}/refresh-market-data")]
    public async Task<IActionResult> RefreshMarketData(int id)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        var property = await _context.Properties.FindAsync(id);
        if (property == null)
        {
            TempData["Error"] = "Property not found.";
            return RedirectToAction("ApprovedListings");
        }

        try
        {
            // Fetch fresh data from RentCast API
            var rentCastData = await _rentCastService.GetValuationAsync(property.Location);

            if (rentCastData != null)
            {
                // Update property with new RentCast data
                property.MarketValue = rentCastData.MarketValue;
                property.RentEstimate = rentCastData.RentEstimate;
                property.YieldScore = rentCastData.CalculateCapRate();
                property.ProfitabilityRating = rentCastData.GetProfitabilityRating();
                property.RentCastLastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Market data refreshed successfully! Cap Rate: {rentCastData.CalculateCapRate():F1}%";
            }
            else
            {
                TempData["Warning"] = "Unable to fetch market data from RentCast API. Please try again later.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error refreshing market data: {ex.Message}";
        }

        return RedirectToAction("ApprovedListingDetail", new { id = property.PropertyId });
    }

    // ==================== NOTIFICATION API ====================

    /// <summary>
    /// Get notifications for the logged-in broker
    /// </summary>
    [HttpGet("api/notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        try
        {
            // Fetch all notifications from the database (no broker filter)
            var notifications = await _context.Notifications
                .Include(n => n.Appointment)
                .OrderByDescending(n => n.CreatedAtUtc)
                .Take(20)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Found {notifications.Count} total notifications in database");

            return Json(new
            {
                success = true,
                notifications = notifications.Select(n => new
                {
                    notificationId = n.NotificationId,
                    title = n.Title,
                    message = n.Message,
                    isRead = n.IsRead,
                    createdAtUtc = n.CreatedAtUtc,
                    notificationType = n.NotificationType,
                    appointmentId = n.AppointmentId
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPost("api/notifications/{id:int}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return Unauthorized(new { success = false, error = "Not authenticated" });

        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && 
                    (n.EmployeeId == brokerId.Value || n.AgentId == brokerId.Value));

            if (notification == null)
                return NotFound(new { success = false, error = "Notification not found" });

            notification.IsRead = true;
            notification.ReadAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    // ==================== CALENDAR API ====================

    /// <summary>
    /// Get appointments for the broker calendar
    /// </summary>
    [HttpGet("api/appointments")]
    public async Task<IActionResult> GetAppointments()
    {
        try
        {
            // Fetch all appointments from the database (no broker filter)
            var appointments = await _context.Appointments
                .Include(a => a.Property)
                .OrderBy(a => a.PreferredDate)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Found {appointments.Count} total appointments in database");

            return Json(new
            {
                success = true,
                appointments = appointments.Select(a => new
                {
                    appointmentId = a.AppointmentId,
                    title = a.AppointmentType == "self"
                        ? $"Self-Visit: {a.Property?.Title ?? "Property"}"
                        : $"Viewing: {a.Property?.Title ?? "Property"}",
                    date = a.PreferredDate.ToString("yyyy-MM-dd"),
                    time = a.PreferredTime,
                    customerName = a.CustomerName,
                    customerEmail = a.CustomerEmail,
                    customerPhone = a.CustomerPhone,
                    status = a.Status,
                    appointmentType = a.AppointmentType
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    // ==================== DEBUG ENDPOINT ====================

    /// <summary>
    /// Get all appointments for debugging (no broker filter)
    /// </summary>
    [HttpGet("api/appointments/all")]
    public async Task<IActionResult> GetAllAppointments()
    {
        try
        {
            var allAppointments = await _context.Appointments
                .Include(a => a.Property)
                .OrderBy(a => a.PreferredDate)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Total appointments in database: {allAppointments.Count}");

            return Json(new
            {
                success = true,
                total = allAppointments.Count,
                appointments = allAppointments.Select(a => new
                {
                    appointmentId = a.AppointmentId,
                    propertyId = a.PropertyId,
                    propertyTitle = a.Property?.Title,
                    agentId = a.AgentId,
                    employeeId = a.EmployeeId,
                    date = a.PreferredDate.ToString("yyyy-MM-dd"),
                    time = a.PreferredTime,
                    customerName = a.CustomerName,
                    status = a.Status,
                    appointmentType = a.AppointmentType
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    // ==================== AGENT PICKUP ====================

    /// <summary>
    /// Agent Pickup page for managing pickup appointments
    /// </summary>
    [HttpGet("agent-pickup")]
    public IActionResult AgentPickup()
    {
        var brokerId = AuthorizationHelper.GetUserId(HttpContext);
        if (!brokerId.HasValue)
            return RedirectToAction("Login", "Admin");

        ViewBag.BrokerName = HttpContext.Session.GetString("UserName") ?? "Broker";
        return View();
    }

    /// <summary>
    /// Get pickup appointments for the broker
    /// </summary>
    [HttpGet("api/pickup-appointments")]
    public async Task<IActionResult> GetPickupAppointments()
    {
        try
        {
            var pickupAppointments = await _context.Appointments
                .Where(a => a.AppointmentType == "pickup")
                .Include(a => a.Property)
                .OrderBy(a => a.PreferredDate)
                .ToListAsync();

            return Json(new
            {
                success = true,
                appointments = pickupAppointments.Select(a => new
                {
                    appointmentId = a.AppointmentId,
                    propertyTitle = a.Property?.Title ?? "Property",
                    customerName = a.CustomerName,
                    customerEmail = a.CustomerEmail,
                    customerPhone = a.CustomerPhone,
                    customerAddress = a.CustomerAddress,
                    preferredDate = a.PreferredDate.ToString("yyyy-MM-dd"),
                    preferredTime = a.PreferredTime,
                    status = a.Status,
                    transportationMethod = a.TransportationMethod,
                    propertyLatitude = a.Property?.Latitude,
                    propertyLongitude = a.Property?.Longitude,
                    customerLatitude = a.CustomerLatitude,
                    customerLongitude = a.CustomerLongitude
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Start a pickup appointment
    /// </summary>
    [HttpPost("api/pickup-appointments/{id:int}/start")]
    public async Task<IActionResult> StartPickup(int id)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound(new { success = false, error = "Appointment not found" });

            appointment.Status = "In Transit";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Pickup started" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Update agent location for tracking
    /// </summary>
    [HttpPost("api/agent/location")]
    public async Task<IActionResult> UpdateAgentLocation([FromBody] LocationUpdateRequest request)
    {
        try
        {
            // Store agent location for tracking
            // This would typically use SignalR for real-time updates
            // For now, we'll just return success
            return Json(new { success = true, message = "Location updated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Get agent location for client tracking
    /// </summary>
    [HttpGet("api/agent/{id:int}/location")]
    public async Task<IActionResult> GetAgentLocation(int id)
    {
        try
        {
            // Return agent location for tracking
            // This would typically use SignalR for real-time updates
            return Json(new
            {
                success = true,
                agentId = id,
                latitude = 0.0,
                longitude = 0.0,
                lastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Store broker location for appointment
    /// </summary>
    [HttpPost("api/pickup-appointments/{appointmentId:int}/location")]
    public async Task<IActionResult> StoreBrokerLocation(int appointmentId, [FromBody] BrokerLocationRequest request)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return Json(new { success = false, error = "Appointment not found" });
            }

            // Store broker location in the appointment
            appointment.BrokerLatitude = request.Latitude;
            appointment.BrokerLongitude = request.Longitude;
            appointment.BrokerLocationLastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Get broker location for appointment
    /// </summary>
    [HttpGet("api/pickup-appointments/{appointmentId:int}/location")]
    public async Task<IActionResult> GetBrokerLocation(int appointmentId)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return Json(new { success = false, error = "Appointment not found" });
            }

            return Json(new
            {
                success = true,
                latitude = appointment.BrokerLatitude,
                longitude = appointment.BrokerLongitude,
                lastUpdated = appointment.BrokerLocationLastUpdated
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}
