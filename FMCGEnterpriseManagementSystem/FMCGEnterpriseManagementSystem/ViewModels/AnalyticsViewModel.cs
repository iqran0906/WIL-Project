
//    Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 08-04-2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

using System;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel used to transfer sales and inventory analytics
    // from the service layer to the analytics dashboard.
    public class AnalyticsViewModel
    {
        // Stores the month labels used for displaying trends.
        public string[] TrendMonths { get; set; } = Array.Empty<string>();

        // Stores the total sales amount for each corresponding month.
        public decimal[] MonthlySalesTotals { get; set; } = Array.Empty<decimal>();

        // Stores the inventory turnover rate for each corresponding month.
        public int[] InventoryTurnoverRates { get; set; } = Array.Empty<int>();
    }
}