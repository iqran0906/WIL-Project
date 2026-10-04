
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<FMCGEnterpriseManagementSystem.Data.ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Services.Interfaces.ISettingsService, FMCGEnterpriseManagementSystem.Services.SettingsService>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.INotificationRepository, FMCGEnterpriseManagementSystem.Repositories.NotificationRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Services.Interfaces.INotificationService, FMCGEnterpriseManagementSystem.Services.NotificationService>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Observers.InventoryNotificationSubject>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Observers.PaymentNotificationSubject>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Observers.EmailNotificationObserver>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Observers.SystemAlertObserver>();
            builder.Services.Configure<FMCGEnterpriseManagementSystem.Models.EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));

            // Invoice
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.IInvoiceRepository, FMCGEnterpriseManagementSystem.Repositories.InvoiceRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.IInventoryRepository, FMCGEnterpriseManagementSystem.Repositories.InventoryRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.IProductRepository, FMCGEnterpriseManagementSystem.Repositories.ProductRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Services.Interfaces.IInvoiceService, FMCGEnterpriseManagementSystem.Services.InvoiceService>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.ICustomerRepository, FMCGEnterpriseManagementSystem.Repositories.CustomerRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Services.Interfaces.IInvoiceExportService, FMCGEnterpriseManagementSystem.Services.InvoiceExportService>();

            // Quote
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.IQuoteRepository, FMCGEnterpriseManagementSystem.Repositories.QuoteRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Services.Interfaces.IQuoteService, FMCGEnterpriseManagementSystem.Services.QuoteService>();

            // Payment
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Repositories.Interfaces.IPaymentRepository, FMCGEnterpriseManagementSystem.Repositories.PaymentRepository>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Services.Interfaces.IPaymentService, FMCGEnterpriseManagementSystem.Services.PaymentService>();
            builder.Services.AddScoped<FMCGEnterpriseManagementSystem.Api.Services.Interfaces.IEmailService,
                            FMCGEnterpriseManagementSystem.Api.Services.EmailService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<FMCGEnterpriseManagementSystem.Api.Middleware.ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
