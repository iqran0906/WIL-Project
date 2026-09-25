using System;
using System.Collections.Generic;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalSales { get; set; }
        public decimal OutstandingPayments { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal ProjectedRestockBudget { get; set; }

        public string[] CategoryNames { get; set; } = Array.Empty<string>();
        public int[] CategoryQuantities { get; set; } = Array.Empty<int>();

        public List<CriticalStockAlert> CriticalStockAlerts { get; set; } = new();
    }

    public class CriticalStockAlert
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public string HealthStatus { get; set; } = string.Empty;
    }
}