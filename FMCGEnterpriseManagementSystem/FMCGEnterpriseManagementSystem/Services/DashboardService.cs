
using System;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public DashboardService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<DashboardViewModel> GetDashboardAnalyticsAsync(int months = 12)
        {
            // Only allow the periods available on the dashboard.
            if (months != 6 && months != 12 && months != 24)
            {
                months = 12;
            }

            var invoices = (await _invoiceRepository.GetAllAsync()).ToList();

            var currentMonth = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            var startMonth = currentMonth.AddMonths(-(months - 1));

            var endDate = currentMonth.AddMonths(1);

            // ---------------------------------------------------------
            // INCOME BY MONTH
            // ---------------------------------------------------------

            var monthlyIncome = Enumerable
                .Range(0, months)
                .Select(offset =>
                {
                    var monthDate = startMonth.AddMonths(offset);

                    var income = invoices
                        .Where(i =>
                            i.InvoiceDate.Year == monthDate.Year &&
                            i.InvoiceDate.Month == monthDate.Month)
                        .Sum(i => i.Total);

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
                    Category = group.Key,

                    Sales = group.Sum(item => item.LineTotal)
                })
                .OrderByDescending(category => category.Sales)
                .ToList();

            return new DashboardViewModel
            {
                Months = months,
                IncomeByMonth = monthlyIncome,
                SalesByCategory = salesByCategory
            };
        }
    }
}