// Title: ASP.NET Core MVC View Models
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // View model containing the information required for a sales VAT report.
    // It represents invoice sales and the VAT calculated for each invoice.
    public class SalesVatReportViewModel
    {
        // Invoice reference number associated with the sale.
        public string InvoiceNumber { get; set; } = string.Empty;

        // Date on which the invoice was issued.
        public DateTime InvoiceDate { get; set; }

        // Customer associated with the invoice.
        public string CustomerName { get; set; } = string.Empty;

        // Sales amount before VAT is included.
        public decimal Subtotal { get; set; }

        // Final invoice amount including applicable VAT.
        public decimal Total { get; set; }

        // VAT amount calculated for the invoice.
        public decimal VatAmount { get; set; }
    }
}
