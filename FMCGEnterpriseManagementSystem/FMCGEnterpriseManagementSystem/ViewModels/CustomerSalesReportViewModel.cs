//    Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 08-04-2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // ViewModel used to transfer customer sales and payment information
    // to the customer sales report.
    public class CustomerSalesReportViewModel
    {
        // Stores the unique identifier of the customer.
        public int CustomerId { get; set; }

        // Stores the customer's name displayed in the report.
        public string CustomerName { get; set; } = string.Empty;

        // Stores the number of invoices associated with the customer.
        public int InvoiceCount { get; set; }

        // Stores the total value of sales made to the customer.
        public decimal TotalSales { get; set; }

        // Stores the total amount that has been paid by the customer.
        public decimal TotalPaid { get; set; }

        // Stores the remaining amount still outstanding for the customer.
        public decimal OutstandingBalance { get; set; }
    }
}