
// Title: ASP.NET Core MVC Views and ViewModels
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // View model containing the information required to generate
    // or display an inventory report.
    public class InventoryReportViewModel
    {
        // Product identification information.
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        // Product category used for reporting and grouping.
        public string Category { get; set; } = string.Empty;

        // Current quantity available in inventory and the
        // minimum quantity that should normally be maintained.
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }

        // Current selling price of the product.
        public decimal SellingPrice { get; set; }

        // Total monetary value represented by the available stock.
        public decimal StockValue { get; set; }

        // Indicates whether the current quantity has reached
        // or fallen below the product's reorder level.
        public bool IsLowStock { get; set; }

        // Stores the earliest known expiry date for stock associated
        // with the product. Nullable because a product may not have
        // an expiry date recorded.
        public DateTime? EarliestExpiryDate { get; set; }
    }
}

