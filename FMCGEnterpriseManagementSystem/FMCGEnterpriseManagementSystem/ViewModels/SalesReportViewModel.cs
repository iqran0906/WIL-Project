/***************************************************************************************
*    Title: Sales Report View Model
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/Reports/SalesReportViewModel.cs
***************************************************************************************/

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