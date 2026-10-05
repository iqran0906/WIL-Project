// Purpose: Application start-up: registers services, database, login (Identity), filters, error pages and routes.
// Authors: iqran0906, Maseeha17, Sayali-St10458649, Naseeha27, ST10068525 (from git history)
// Uses: QuestPDF (QuestPDF Community License) https://www.questpdf.com
// Uses: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Filters;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers;
using FMCGEnterpriseManagementSystem.Repositories;
using FMCGEnterpriseManagementSystem.Repositories.Implementations;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.Strategies;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// QuestPDF Community License
QuestPDF.Settings.License = LicenseType.Community;

// MVC
builder.Services.AddControllersWithViews(options =>
{
    // Friendly message + logging when a form submission fails unexpectedly
    options.Filters.Add<GlobalExceptionFilter>();

    // Records successful actions for the Recent Activity page
    options.Filters.Add<ActivityLogFilter>();
});

// Register existing repositories and services
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Dashboard & Analytics Services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<IUserAccountService, UserAccountService>();

builder.Services.AddHttpClient<FMCGEnterpriseManagementSystem.Services.Interfaces.IEmailApiClientService,
                                FMCGEnterpriseManagementSystem.Services.EmailApiClientService>(client =>
                                {
                                    client.BaseAddress = new Uri("https://localhost:7194/");
                                });
builder.Services.AddScoped<IExportStrategy, PdfExportStrategy>();
builder.Services.AddScoped<IExportStrategy, ExcelExportStrategy>();
builder.Services.AddScoped<ExportFactory>();

builder.Services.AddScoped<ISalesRepresentativeRepository, SalesRepresentativeRepository>();
builder.Services.AddScoped<ISalesRepresentativeService, SalesRepresentativeService>();

// ==========================================================
// DATABASE
// ==========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IForecastingRepository, ForecastingRepository>();
builder.Services.AddScoped<IForecastingService, ForecastingService>();

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// ==========================================================
// IDENTITY / AUTHENTICATION
// ==========================================================

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// ==========================================================
// INVENTORY MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IInventoryService, InventoryService>();

// ==========================================================
// GLOBAL SEARCH
// ==========================================================

builder.Services.AddScoped<ISearchService, SearchService>();

// ==========================================================
// SYSTEM SETTINGS
// ==========================================================

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISettingsService, SettingsService>();

// ==========================================================
// RECENT ACTIVITY
// ==========================================================

builder.Services.AddScoped<IActivityService, ActivityService>();

// ==========================================================
// EXPORTS
// ==========================================================

builder.Services.AddScoped<IInvoiceExportService, InvoiceExportService>();

// ==========================================================
// NOTIFICATIONS
// ==========================================================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<InventoryNotificationSubject>();
builder.Services.AddScoped<PaymentNotificationSubject>();

builder.Services.AddScoped<EmailNotificationObserver>();
builder.Services.AddScoped<SystemAlertObserver>();

// ==========================================================
// BUILD APPLICATION
// ==========================================================

var app = builder.Build();
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// ==========================================================
// ERROR HANDLING / SECURITY
// ==========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCodePage", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ==========================================================
// ROUTING
// ==========================================================

app.MapControllerRoute(
    name: "login",
    pattern: "",
    defaults: new
    {
        controller = "Account",
        action = "Login"
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==========================================================
// SEED ROLES / INITIAL USERS
// ==========================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Execute existing SeedData
        await SeedData.InitializeAsync(services, builder.Configuration);

        // Explicit Admin Account Fallback Seeding
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var adminEmail = "admin@fmcg.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(newAdmin, "Wholesale101@");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding initial roles and admin user.");
    }
}

// ==========================================================
// RUN APPLICATION
// ==========================================================

app.Run();
