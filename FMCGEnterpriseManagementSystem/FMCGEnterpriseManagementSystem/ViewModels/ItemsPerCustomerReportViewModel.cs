namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class ItemsPerCustomerReportViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public int TotalItemsPurchased { get; set; }
        public decimal TotalSales { get; set; }
    }
}