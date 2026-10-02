// Purpose: One row of the customer sales report.
// Authors: iqran0906 (from git history)

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class CustomerSalesReportViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int InvoiceCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
}