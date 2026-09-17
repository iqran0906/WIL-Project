namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        // Summary Cards
        public int TotalProducts { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalCustomers { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }

        // Analytics & Projections
        public decimal TotalInventoryValue { get; set; }
        public decimal ProjectedRestockBudget { get; set; }
        public List<CategoryStockSummary> CategorySummaries { get; set; } = new();
        public List<LowStockAlertItem> CriticalStockAlerts { get; set; } = new();
    }

    public class CategoryStockSummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class LowStockAlertItem
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public string HealthStatus { get; set; } = string.Empty; // "Critical" or "Warning"
    }
}