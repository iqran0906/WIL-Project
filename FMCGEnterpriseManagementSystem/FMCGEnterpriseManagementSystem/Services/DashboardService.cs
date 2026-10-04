// Title: Asynchronous programming with async and await
// Author: Maseeha17
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for calculating and providing dashboard analytics.
    public class DashboardService : IDashboardService
    {
        // Repository used to retrieve invoice information.
        private readonly IInvoiceRepository _invoiceRepository;

        // Initialises the dashboard service with the invoice repository.
        public DashboardService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // Calculates dashboard analytics for the selected number of months.
        public async Task<DashboardViewModel> GetDashboardAnalyticsAsync(int months = 12)
        {
            // Only allow the periods available on the dashboard.
            // Any unsupported value is replaced with the default 12-month period.
            if (months != 6 && months != 12 && months != 24)
            {
                months = 12;
            }

            // Retrieves all invoices and converts the result into a list for processing.
            var invoices = (await _invoiceRepository.GetAllAsync()).ToList();

            // Determines the first day of the current month.
            var currentMonth = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            // Calculates the first month included in the selected reporting period.
            var startMonth = currentMonth.AddMonths(-(months - 1));

            // Calculates the first day after the current month for the reporting range.
            var endDate = currentMonth.AddMonths(1);

            // ---------------------------------------------------------
            // INCOME BY MONTH
            // ---------------------------------------------------------

            // Creates monthly income information for each month in the selected period.
            var monthlyIncome = Enumerable
                .Range(0, months)
                .Select(offset =>
                {
                    // Calculates the date represented by the current month in the range.
                    var monthDate = startMonth.AddMonths(offset);

                    // Calculates the total invoice value for the selected month.
                    var income = invoices
                        .Where(i =>
                            i.InvoiceDate.Year == monthDate.Year &&
                            i.InvoiceDate.Month == monthDate.Month)
                        .Sum(i => i.Total);

                    // Creates a monthly income record for the dashboard.
                    return new MonthlyIncome
                    {
                        Year = monthDate.Year,
                        Month = monthDate.Month,
                        Label = monthDate.ToString("MMM yyyy"),
                        Income = income
                    };
                })
                .ToList();

            // ---------------------------------------------------------
            // SALES BY PRODUCT CATEGORY
            // ---------------------------------------------------------

            // Filters invoices to the selected reporting period and groups
            // their invoice items according to the product category.
            var salesByCategory = invoices
                .Where(i =>
                    i.InvoiceDate >= startMonth &&
                    i.InvoiceDate < endDate)
                .SelectMany(i => i.InvoiceItems)
                .Where(item => item.Product != null)
                .GroupBy(item =>
                    string.IsNullOrWhiteSpace(item.Product.Category)
                        ? "Unassigned"
                        : item.Product.Category)
                .Select(group => new CategorySales
                {
                    // Stores the product category name.
                    Category = group.Key,

                    // Calculates the total sales value for the category.
                    Sales = group.Sum(item => item.LineTotal)
                })
                // Displays categories with the highest sales first.
                .OrderByDescending(category => category.Sales)
                .ToList();

            // Returns the calculated analytics to the dashboard.
            return new DashboardViewModel
            {
                Months = months,
                IncomeByMonth = monthlyIncome,
                SalesByCategory = salesByCategory
            };
        }
    }
}