using System;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class AnalyticsViewModel
    {
        public string[] TrendMonths { get; set; } = Array.Empty<string>();
        public decimal[] MonthlySalesTotals { get; set; } = Array.Empty<decimal>();
        public int[] InventoryTurnoverRates { get; set; } = Array.Empty<int>();
    }
}