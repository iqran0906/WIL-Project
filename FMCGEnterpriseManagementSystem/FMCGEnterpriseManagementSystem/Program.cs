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
using System.Linq;
using System.Security.Claims;
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

// ==========================================================
// API HTTP CLIENT CONFIGURATION
// ==========================================================
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddHttpClient<FMCGEnterpriseManagementSystem.Services.Interfaces.IEmailApiClientService,
                                FMCGEnterpriseManagementSystem.Services.EmailApiClientService>(client =>
                                {
                                    client.BaseAddress = new Uri(apiBaseUrl!);
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
// SEED ROLES & COMPLETE ADMIN ACCESS
// ==========================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Execute existing SeedData initialization
        await SeedData.InitializeAsync(services, builder.Configuration);

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Create all standard application roles
        string[] allRoles = { "Admin", "Administrator", "Manager", "Employee", "SalesRepresentative" };
        foreach (var role in allRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@fmcg.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, "Wholesale101@");
            if (createResult.Succeeded)
            {
                foreach (var role in allRoles)
                {
                    await userManager.AddToRoleAsync(adminUser, role);
                }
            }
        }
        else
        {
            // Force reset password to ensure credentials match Wholesale101@
            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            await userManager.ResetPasswordAsync(adminUser, token, "Wholesale101@");

            // Guarantee account active and verified status
            adminUser.EmailConfirmed = true;
            adminUser.IsActive = true;
            await userManager.UpdateAsync(adminUser);

            // Grant ALL roles to guarantee complete access to all protected routes
            foreach (var role in allRoles)
            {
                if (!await userManager.IsInRoleAsync(adminUser, role))
                {
                    await userManager.AddToRoleAsync(adminUser, role);
                }
            }
        }

        // Grant explicit admin claims if policy-based authorization is used
        if (adminUser != null)
        {
            var existingClaims = await userManager.GetClaimsAsync(adminUser);
            if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == "FullAccess"))
            {
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "FullAccess"));
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding full admin permissions.");
    }
}

// ==========================================================
// RUN APPLICATION
// ==========================================================

app.Run();