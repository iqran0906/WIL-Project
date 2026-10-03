/***************************************************************************************
*    Title: Item Sales Report View Model
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/Reports/ItemSalesReportViewModel.cs
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    public class ItemSalesReportViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
    }
}