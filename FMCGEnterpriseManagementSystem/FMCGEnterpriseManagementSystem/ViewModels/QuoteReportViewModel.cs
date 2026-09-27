namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class QuoteReportViewModel
    {
        public int QuoteId { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public DateTime QuoteDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
    }
}