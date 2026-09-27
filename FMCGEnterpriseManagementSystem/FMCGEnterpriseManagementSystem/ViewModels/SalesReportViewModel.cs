namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class SalesReportViewModel
    {
        public DateTime SaleDate { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
}