using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Data.Seed;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Implementations;
using TourManagement.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.LogoutPath = "/Account/Logout";
    opt.AccessDeniedPath = "/Account/AccessDenied";
    opt.ExpireTimeSpan = TimeSpan.FromHours(8);
    opt.SlidingExpiration = true;
});

// Services
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IGuideService, GuideService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        context.Database.Migrate();
        await DbInitializer.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data error: {Message}", ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();