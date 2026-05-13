using RealEstate.Models;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.Services;

/// <summary>
/// Service for seeding initial data into the comprehensive database
/// </summary>
public class DataSeeder
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ApplicationDBContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seed all initial data
    /// </summary>
    public async Task SeedAll()
    {
        try
        {
            _logger.LogInformation("Starting data seeding...");

            await SeedPredefinedUsers();
            await SeedPropertyTypes();
            await SeedDepartmentsAndEmployees();
            await SeedCommissionRules();
            await SeedAuditLogs();
            await SeedAgents();
            await SeedCustomers();
            
            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data seeding");
            throw;
        }
    }

    /// <summary>
    /// Seed default commission rules for managers
    /// </summary>
    private async Task SeedCommissionRules()
    {
        if (await _context.CommissionRules.AnyAsync())
        {
            _logger.LogInformation("Commission rules already exist, skipping");
            return;
        }

        _logger.LogInformation("Seeding commission rules...");

        var managers = await _context.Users
            .Where(u => u.Role == "Manager")
            .ToListAsync();

        if (!managers.Any())
        {
            _logger.LogWarning("No managers found, cannot seed commission rules");
            return;
        }

        var commissionRules = new List<CommissionRule>();

        foreach (var manager in managers)
        {
            commissionRules.Add(new CommissionRule
            {
                ManagerId = manager.UserId,
                CommissionPercent = 3.0m,  // 3% agent commission
                CompanySharePercent = 2.0m, // 2% company share
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.CommissionRules.AddRangeAsync(commissionRules);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {commissionRules.Count} commission rules");
    }

    /// <summary>
    /// Seed initial audit log entry
    /// </summary>
    private async Task SeedAuditLogs()
    {
        if (await _context.AuditLogs.AnyAsync())
        {
            _logger.LogInformation("Audit logs already exist, skipping");
            return;
        }

        _logger.LogInformation("Seeding initial audit log...");

        var adminUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Role == "SuperAdmin");

        if (adminUser != null)
        {
            await _context.AuditLogs.AddAsync(new AuditLog
            {
                UserId = adminUser.UserId,
                UserRole = "SuperAdmin",
                Action = "System Initialization",
                EntityType = "System",
                Description = "Comprehensive database schema initialized",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("Initial audit log seeded");
        }
    }

    /// <summary>
    /// Seed predefined user accounts for each role: Manager, Broker, Seller, Accounting
    /// </summary>
    private async Task SeedPredefinedUsers()
    {
        _logger.LogInformation("Checking predefined user accounts...");

        var predefinedUsers = new[]
        {
            new { FullName = "System Manager",    Username = "manager",    Email = "manager@estateflow.com",    Password = "Manager@123",    Role = "Manager"    },
            new { FullName = "System Broker",     Username = "broker",     Email = "broker@estateflow.com",     Password = "Broker@123",     Role = "Broker"     },
            new { FullName = "System Seller",     Username = "seller",     Email = "seller@estateflow.com",     Password = "Seller@123",     Role = "Seller"     },
            new { FullName = "System Accounting", Username = "accounting", Email = "accounting@estateflow.com", Password = "Accounting@123", Role = "Accounting" },
            new { FullName = "Juan Luna",         Username = "agent",      Email = "juan.luna@estateflow.com",  Password = "Agent@123",      Role = "Agent"      },
        };

        int seededCount = 0;

        foreach (var u in predefinedUsers)
        {
            bool exists = await _context.Users.AnyAsync(x => x.Username == u.Username || x.Email == u.Email);
            if (exists)
            {
                _logger.LogInformation($"User '{u.Username}' already exists, skipping");
                continue;
            }

            var user = new ApplicationUser
            {
                FullName    = u.FullName,
                Username    = u.Username,
                Email       = u.Email,
                PasswordHash = HashPassword(u.Password),
                Role        = u.Role,
                Status      = "Active",
                IsActive    = true,
                CreatedAt   = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            seededCount++;
        }

        if (seededCount > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {seededCount} predefined user account(s)");
        }
        else
        {
            _logger.LogInformation("All predefined user accounts already exist");
        }
    }

    /// <summary>
    /// Hash a plain-text password using SHA-256 (mirrors UserService.HashPassword)
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha256.ComputeHash(bytes));
    }

    /// <summary>
    /// Create sample properties for testing (optional)
    /// Call this method explicitly when needed
    /// </summary>
    public async Task SeedSampleProperties()
    {
        int currentCount = await _context.Properties.CountAsync();
        if (currentCount >= 50)
        {
            _logger.LogInformation($"Already have {currentCount} properties, skipping mass seed.");
            return;
        }

        _logger.LogInformation("Seeding mass sample properties...");

        var sellers = await _context.Users
            .Where(u => u.Role == "Seller")
            .ToListAsync();

        if (!sellers.Any())
        {
            _logger.LogWarning("No sellers found, cannot seed sample properties");
            return;
        }

        var sellerUser = sellers.First();
        
        // Ensure PropertySeller exists
        var propertySeller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == sellerUser.UserId);
        if (propertySeller == null)
        {
            propertySeller = new PropertySeller
            {
                UserId = sellerUser.UserId,
                SellerName = sellerUser.FullName,
                Rating = 5,
                IdentityVerified = true
            };
            _context.Sellers.Add(propertySeller);
            await _context.SaveChangesAsync();
        }

        var sampleProperties = new List<Property>();
        var locations = new[] { "Makati City", "Bonifacio Global City", "Quezon City", "Pasig City", "Alabang, Muntinlupa", "Cebu City", "Davao City", "Tagaytay City", "Laiya, Batangas", "Baguio City" };
        var types = new[] { "House", "Condo", "Villa", "Townhouse", "Commercial" };
        var statuses = new[] { "Approved", "Pending", "InspectionApproved", "InspectionContactSeller" };
        var images = new[] 
        {
            "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1583608205776-bfd35f0d9f83?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1600607687920-4e2a09c2640c?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1513584684374-8bab748fbf90?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1570129477492-45c003edd2be?auto=format&fit=crop&w=800&q=80"
        };
        var rand = new Random();

        for (int i = 1; i <= 50; i++)
        {
            string loc = locations[rand.Next(locations.Length)];
            string type = types[rand.Next(types.Length)];
            string img = images[rand.Next(images.Length)];
            
            sampleProperties.Add(new Property
            {
                SellerId = propertySeller.SellerId,
                Title = $"Beautiful {type} in {loc} ({i})",
                Description = $"A stunning and spacious {type.ToLower()} located in the heart of {loc}. Features modern amenities, great views, and easy access to local establishments.",
                PropertyType = type,
                Location = loc,
                BasePrice = rand.Next(5, 50) * 1000000m, // 5M to 50M
                Status = statuses[rand.Next(statuses.Length)],
                Bedrooms = rand.Next(1, 6),
                Bathrooms = rand.Next(1, 4),
                ParkingSlots = rand.Next(0, 3),
                Sqft = rand.Next(50, 500),
                CoverImage = img,
                CreatedAt = DateTime.UtcNow.AddDays(-rand.Next(1, 60)) // Random past 60 days
            });
        }

        await _context.Properties.AddRangeAsync(sampleProperties);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {sampleProperties.Count} mass sample properties");
    }

    /// <summary>
    /// Create sample inquiries for testing (optional)
    /// </summary>
    public async Task SeedSampleInquiries()
    {
        if (await _context.Inquiries.AnyAsync())
        {
            _logger.LogInformation("Inquiries already exist, skipping");
            return;
        }

        var properties = await _context.Properties.Take(3).ToListAsync();
        if (!properties.Any())
        {
            _logger.LogWarning("No properties found, cannot seed inquiries");
            return;
        }

        var sampleInquiries = new List<PropertyInquiry>
        {
            new PropertyInquiry
            {
                PropertyId = properties[0].PropertyId,
                CustomerName = "Juan Dela Cruz",
                CustomerEmail = "juan@example.com",
                CustomerPhone = "+63 917 123 4567",
                Message = "I'm interested in this property. Is it still available? Can I schedule a viewing?",
                Status = InquiryStatus.New,
                CreatedAt = DateTime.UtcNow
            },
            new PropertyInquiry
            {
                PropertyId = properties.Count > 1 ? properties[1].PropertyId : properties[0].PropertyId,
                CustomerName = "Maria Santos",
                CustomerEmail = "maria@example.com",
                CustomerPhone = "+63 918 234 5678",
                Message = "What are the payment terms? Do you accept bank financing?",
                Status = InquiryStatus.New,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Inquiries.AddRangeAsync(sampleInquiries);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {sampleInquiries.Count} sample inquiries");
    }

    private async Task SeedPropertyTypes()
    {
        if (await _context.PropertyTypes.AnyAsync()) return;

        _logger.LogInformation("Seeding Property Types...");
        var types = new List<PropertyType>
        {
            new PropertyType { TypeName = "House" },
            new PropertyType { TypeName = "Condo" },
            new PropertyType { TypeName = "Apartment" },
            new PropertyType { TypeName = "Land" },
            new PropertyType { TypeName = "Commercial" }
        };
        await _context.PropertyTypes.AddRangeAsync(types);
        await _context.SaveChangesAsync();
    }

    private async Task SeedAgents()
    {
        int currentCount = await _context.Agents.CountAsync();
        if (currentCount >= 50) return;

        _logger.LogInformation($"Seeding Agents... Current count: {currentCount}. Generating up to 50.");
        
        // Seed the default system agents if they don't exist
        if (currentCount == 0)
        {
            var agentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "juan.luna@estateflow.com");

            var initialAgents = new List<Agent>
            {
                new Agent
                {
                    UserId = agentUser?.UserId,
                    FirstName = "Juan", LastName = "Luna", Email = "juan.luna@estateflow.com",
                    PhoneNumber = "+63 917 111 2222", YearsOfExperience = 5,
                    LicenseNumber = "REB-12345", Title = "Senior Agent", Name = "Juan Luna"
                },
                new Agent
                {
                    FirstName = "Maria", LastName = "Clara", Email = "maria.clara@estateflow.com",
                    PhoneNumber = "+63 918 333 4444", YearsOfExperience = 3,
                    LicenseNumber = "REB-67890", Title = "Broker", Name = "Maria Clara"
                }
            };
            await _context.Agents.AddRangeAsync(initialAgents);
            await _context.SaveChangesAsync();
            currentCount = await _context.Agents.CountAsync();
        }

        var agentsToSeed = 50 - currentCount;
        if (agentsToSeed <= 0) return;

        var random = new Random();
        var firstNames = new[] { "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Kenneth", "Kevin", "Brian", "George" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson" };
        var titles = new[] { "Agent", "Senior Agent", "Broker", "Associate Broker", "Property Consultant" };
        
        var generatedAgents = new List<Agent>();

        for (int i = 0; i < agentsToSeed; i++)
        {
            var fName = firstNames[random.Next(firstNames.Length)];
            var lName = lastNames[random.Next(lastNames.Length)];
            var title = titles[random.Next(titles.Length)];
            
            var photoId = random.Next(1, 70); 
            var genderStr = random.Next(2) == 0 ? "men" : "women";
            var photoUrl = $"https://randomuser.me/api/portraits/{genderStr}/{photoId}.jpg";

            generatedAgents.Add(new Agent
            {
                FirstName = fName,
                LastName = lName,
                Email = $"{fName.ToLower()}.{lName.ToLower()}{random.Next(1, 1000)}@estateflow.com",
                PhoneNumber = $"+63 9{random.Next(10, 99)} {random.Next(100, 999)} {random.Next(1000, 9999)}",
                YearsOfExperience = random.Next(1, 15),
                LicenseNumber = $"REB-{random.Next(10000, 99999)}",
                Title = title,
                Name = $"{fName} {lName}",
                PhotoUrl = photoUrl,
                ProfilePhotoPath = photoUrl,
                Bio = $"Experienced {title.ToLower()} specializing in residential and commercial properties. Dedicated to finding the perfect match for my clients.",
                IsArchived = false,
                CreatedAtUtc = DateTime.UtcNow
            });
        }
        
        await _context.Agents.AddRangeAsync(generatedAgents);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Successfully seeded {generatedAgents.Count} additional agents.");
    }

    private async Task SeedCustomers()
    {
        if (await _context.Customers.AnyAsync()) return;

        _logger.LogInformation("Seeding Customers...");
        var customers = new List<Customer>
        {
            new Customer
            {
                FullName = "Jose Rizal", Email = "jose.rizal@example.com",
                Phone = "+63 919 555 6666", City = "Manila", PropertyType = "House",
                MinBudget = 5000000, MaxBudget = 15000000, Status = "Interested"
            },
            new Customer
            {
                FullName = "Andres Bonifacio", Email = "andres.b@example.com",
                Phone = "+63 920 777 8888", City = "Quezon City", PropertyType = "Commercial",
                MinBudget = 10000000, MaxBudget = 50000000, Status = "Viewing Scheduled"
            }
        };
        await _context.Customers.AddRangeAsync(customers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedDepartmentsAndEmployees()
    {
        if (await _context.Departments.AnyAsync()) return;

        _logger.LogInformation("Seeding Departments and Employees...");
        var salesDept = new Department { DeptName = "Sales", CreatedAt = DateTime.UtcNow };
        var hrDept = new Department { DeptName = "Human Resources", CreatedAt = DateTime.UtcNow };
        
        await _context.Departments.AddRangeAsync(salesDept, hrDept);
        await _context.SaveChangesAsync();

        var accountingUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Accounting");
        if (accountingUser != null && !await _context.Employees.AnyAsync(e => e.UserId == accountingUser.UserId))
        {
            var employee = new Employee
            {
                UserId = accountingUser.UserId,
                DeptId = hrDept.DeptId,
                Salary = 45000m,
                HireDate = DateTime.UtcNow.AddYears(-1),
                Status = "Active"
            };
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }
    }
}
