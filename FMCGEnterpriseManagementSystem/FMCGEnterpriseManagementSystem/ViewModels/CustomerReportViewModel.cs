/***************************************************************************************
*    Title: Customer Report View Model
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/Reports/CustomerReportViewModel.cs
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class CustomerReportViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TelephoneNumber { get; set; } = string.Empty;
        public string CustomerGroup { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}