namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class InvoiceReportViewModel
    {
        public int InvoiceId { get; set; } 
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
}