using Microsoft.EntityFrameworkCore;
using RealEstate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<RealEstate.Services.PropertyCatalog>();
builder.Services.AddSingleton<RealEstate.Services.InquiryService>();
builder.Services.AddSingleton<RealEstate.Services.SubscriptionService>();
builder.Services.AddScoped<RealEstate.Services.AppointmentService>();
builder.Services.AddScoped<RealEstate.Services.UserService>();
// Investor controllers use the same services; no extra DI required

// Database context for admin authentication and appointments
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

// Root route for landing page
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" })
    .WithStaticAssets();

// Default fallback route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
