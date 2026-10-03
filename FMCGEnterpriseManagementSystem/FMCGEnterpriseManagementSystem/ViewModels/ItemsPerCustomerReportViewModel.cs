/***************************************************************************************
*    Title: Items Per Customer Report View Model
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/Reports/ItemsPerCustomerReportViewModel.cs
***************************************************************************************/

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