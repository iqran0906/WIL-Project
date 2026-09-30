// Purpose: One row of the sales vat report.
// Authors: iqran0906 (from git history)

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class SalesVatReportViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal VatAmount { get; set; }
    }
}