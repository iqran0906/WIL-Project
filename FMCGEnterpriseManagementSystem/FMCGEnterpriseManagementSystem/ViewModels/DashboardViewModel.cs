/***************************************************************************************
*    Title: Dashboard View Model
*    Author: ST10068525, Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/DashboardViewModel.cs
***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        // Number of months displayed on the dashboard
        public int Months { get; set; } = 12;

        // Income chart
        public List<MonthlyIncome> IncomeByMonth { get; set; } = new();

        // Sales by product category chart
        public List<CategorySales> SalesByCategory { get; set; } = new();

        public decimal TotalIncome =>
            IncomeByMonth.Sum(m => m.Income);

        public decimal TotalCategorySales =>
            SalesByCategory.Sum(c => c.Sales);

        public decimal ThisMonthIncome =>
            IncomeByMonth.LastOrDefault()?.Income ?? 0;

        public decimal LastMonthIncome =>
            IncomeByMonth.Count > 1
                ? IncomeByMonth[^2].Income
                : 0;

        public decimal? MonthOnMonthChange =>
            LastMonthIncome == 0
                ? null
                : (ThisMonthIncome - LastMonthIncome)
                  / LastMonthIncome * 100m;
    }

    public class MonthlyIncome
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public string Label { get; set; } = string.Empty;

        public decimal Income { get; set; }
    }

    public class CategorySales
    {
        public string Category { get; set; } = string.Empty;

        public decimal Sales { get; set; }
    }
}