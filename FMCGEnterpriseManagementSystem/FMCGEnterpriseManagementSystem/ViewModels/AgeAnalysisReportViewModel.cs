// Purpose: One row of the age analysis report.
// Authors: iqran0906 (from git history)

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class AgeAnalysisReportViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int AgeInDays { get; set; }
        public decimal InvoiceTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal OutstandingBalance { get; set; }
        public string AgeBracket { get; set; } = string.Empty;
    }
}