namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class SalesRepReportViewModel
    {
        public int SalesRepresentativeId { get; set; }
        public string SalesRepCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public decimal SalesTarget { get; set; }
        public decimal TotalSales { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal EstimatedCommission { get; set; }
    }
}