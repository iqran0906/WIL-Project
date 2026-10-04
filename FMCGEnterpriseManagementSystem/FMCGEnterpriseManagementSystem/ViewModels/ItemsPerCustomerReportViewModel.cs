//   Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // ViewModel used to transfer customer purchasing information
    // to the items-per-customer report.
    public class ItemsPerCustomerReportViewModel
    {
        // Stores the unique identifier of the customer.
        public int CustomerId { get; set; }

        // Stores the customer's name displayed in the report.
        public string CustomerName { get; set; } = string.Empty;

        // Stores the total number of individual items purchased by the customer.
        public int TotalItemsPurchased { get; set; }

        // Stores the total sales value generated from the customer's purchases.
        public decimal TotalSales { get; set; }
    }
}