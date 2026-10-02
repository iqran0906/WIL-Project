// Purpose: One row of the item sales report.
// Authors: iqran0906 (from git history)

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class ItemSalesReportViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
    }
}