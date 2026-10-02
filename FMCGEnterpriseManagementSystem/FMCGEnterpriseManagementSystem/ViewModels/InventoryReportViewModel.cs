// Purpose: One row of the inventory report.
// Authors: iqran0906 (from git history)

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class InventoryReportViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }

        public decimal SellingPrice { get; set; }
        public decimal StockValue { get; set; }

        public bool IsLowStock { get; set; }

        public DateTime? EarliestExpiryDate { get; set; }
    }
}