using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Factories;
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
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// QuestPDF Community License
QuestPDF.Settings.License = LicenseType.Community;

// MVC
builder.Services.AddControllersWithViews();


// ==========================================================
// DATABASE
// ==========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


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
// AUTHENTICATION / USER ACCOUNTS
// ==========================================================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();


// ==========================================================
// EMPLOYEE MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();


// ==========================================================
// SALES REPRESENTATIVE MANAGEMENT
// ==========================================================

builder.Services.AddScoped<ISalesRepresentativeRepository, SalesRepresentativeRepository>();
builder.Services.AddScoped<ISalesRepresentativeService, SalesRepresentativeService>();


// ==========================================================
// CUSTOMER MANAGEMENT
// ==========================================================

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();


// ==========================================================
// SUPPLIER MANAGEMENT
// ==========================================================

builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();


// ==========================================================
// PRODUCT MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();


// ==========================================================
// INVENTORY MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();


// ==========================================================
// FORECASTING
// ==========================================================

builder.Services.AddScoped<IForecastingService, ForecastingService>();


// ==========================================================
// QUOTE MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IQuoteService, QuoteService>();


// ==========================================================
// INVOICE MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();


// ==========================================================
// PAYMENT MANAGEMENT
// ==========================================================

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


// ==========================================================
// REPORTING
// ==========================================================

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();


// ==========================================================
// EXPORTS
// ==========================================================

builder.Services.AddScoped<IExportStrategy, PdfExportStrategy>();
builder.Services.AddScoped<IExportStrategy, ExcelExportStrategy>();
builder.Services.AddScoped<ExportFactory>();


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


// ==========================================================
// ERROR HANDLING / SECURITY
// ==========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


// ==========================================================
// ROUTING
// ==========================================================

// Application root opens Login
app.MapControllerRoute(
    name: "login",
    pattern: "",
    defaults: new
    {
        controller = "Account",
        action = "Login"
    });

// Standard MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ==========================================================
// SEED ROLES / INITIAL USERS
// ==========================================================

using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(
        scope.ServiceProvider,
        builder.Configuration);
}


// ==========================================================
// RUN APPLICATION
// ==========================================================

app.Run();