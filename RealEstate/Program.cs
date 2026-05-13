using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate;
using RealEstate.Hubs;
using RealEstate.Services;

var builder = WebApplication.CreateBuilder(args);

// Allow large file uploads (resume + valid ID – up to 100 MB per request)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000; // 100 MB
});

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddControllersWithViews()
    .AddCookieTempDataProvider(options => {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<PropertyCatalog>();
builder.Services.AddSingleton<InquiryService>();
builder.Services.AddSingleton<SubscriptionService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<DataSeeder>();

// Register RentCast Service with HttpClient
builder.Services.AddHttpClient<RentCastService>(client =>
{
    client.BaseAddress = new Uri("https://api.rentcast.io/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add SignalR for real-time video calling
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
    options.StreamBufferCapacity = 50;
});

// Register Weather Forecast Service with HttpClient
builder.Services.AddHttpClient<WeatherForecastService>(client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Register LocationIQ Geocoding Service with HttpClient
builder.Services.AddHttpClient<GeocodingService>(client =>
{
    client.BaseAddress = new Uri("https://us1.locationiq.com/v1/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// PayMongo Service Registration
var payMongoSecretKey = builder.Configuration["PayMongo:SecretKey"];
var payMongoPublicKey = builder.Configuration["PayMongo:PublicKey"];
builder.Services.AddScoped(provider => new PayMongoService(payMongoSecretKey, payMongoPublicKey));

// Add Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Database context for admin authentication and appointments
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAll();

        // Seed inspection items
        var context = services.GetRequiredService<ApplicationDBContext>();
        await RealEstate.Services.InspectionItemSeeder.SeedInspectionItemsAsync(context);

        // Optional: Seed sample data for testing
        await seeder.SeedSampleProperties();
        await seeder.SeedSampleInquiries();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during data seeding.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Replaces app.MapStaticAssets()
app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hub for video calling
app.MapHub<CallHub>("/callHub");

// Root route for landing page
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" });

// Default fallback route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();