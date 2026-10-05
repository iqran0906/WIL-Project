// Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // ViewModel used to transfer inventory information to the inventory report.
    public class InventoryReportViewModel
    {
        // Stores the unique identifier of the product.
        public int ProductId { get; set; }

        // Stores the product code displayed in the report.
        public string ProductCode { get; set; } = string.Empty;

        // Stores the name of the product displayed in the report.
        public string ProductName { get; set; } = string.Empty;

        // Stores the category assigned to the product.
        public string Category { get; set; } = string.Empty;

        // Stores the current quantity of the product available in stock.
        public int QuantityOnHand { get; set; }

        // Stores the stock quantity at which the product should be reordered.
        public int ReorderLevel { get; set; }

        // Stores the current selling price of the product.
        public decimal SellingPrice { get; set; }

        // Stores the total value of the available stock.
        public decimal StockValue { get; set; }

        // Indicates whether the current stock quantity has reached or fallen
        // below the configured reorder level.
        public bool IsLowStock { get; set; }

        // Stores the earliest expiry date among the available stock batches.
        // Nullable because the product may not have an expiry date.
        public DateTime? EarliestExpiryDate { get; set; }
    }
}