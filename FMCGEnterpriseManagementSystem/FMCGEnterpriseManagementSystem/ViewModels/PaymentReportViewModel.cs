namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class PaymentReportViewModel
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
    }
}