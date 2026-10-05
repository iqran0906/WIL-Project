
// Title: ASP.NET Core MVC View Models
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/using-type-dynamic

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // View model used to present aggregated sales information by date.
    public class SalesReportViewModel
    {
        // Date represented by the sales report entry.
        public DateTime SaleDate { get; set; }

        // Number of invoices recorded for the date.
        public int InvoiceCount { get; set; }

        // Total sales value recorded for the date.
        public decimal TotalSales { get; set; }

        // Total amount received from customers for the date.
        public decimal TotalPaid { get; set; }

        // Remaining amount owed by customers.
        public decimal OutstandingBalance { get; set; }
    }
}
