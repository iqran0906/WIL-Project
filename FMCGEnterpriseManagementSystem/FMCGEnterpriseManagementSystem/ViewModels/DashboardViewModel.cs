// Purpose: Data for the dashboard charts (income per month, sales per category).
// Authors: ST10068525 (new file, not yet committed)

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        // How many months the charts cover (6, 12 or 24)
        public int Months { get; set; } = 12;

        public List<MonthlyIncome> IncomeByMonth { get; set; } = new();

        public List<CategorySales> SalesByCategory { get; set; } = new();

        public decimal TotalIncome => IncomeByMonth.Sum(m => m.Income);

        public decimal TotalCategorySales => SalesByCategory.Sum(c => c.Sales);

        public decimal ThisMonthIncome => IncomeByMonth.LastOrDefault()?.Income ?? 0;

        public decimal LastMonthIncome =>
            IncomeByMonth.Count > 1 ? IncomeByMonth[^2].Income : 0;

        // Month-on-month change in %, or null when last month had no income
        public decimal? MonthOnMonthChange =>
            LastMonthIncome == 0 ? null : (ThisMonthIncome - LastMonthIncome) / LastMonthIncome * 100m;
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
