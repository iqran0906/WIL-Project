//   Title: Classes and Objects - C# Programming Guide
//   Author: Microsoft
//   Date: 2026
//   Code version: C#
//   Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // ViewModel used to transfer invoice information to the invoice report.
    public class InvoiceReportViewModel
    {
        // Stores the unique identifier of the invoice.
        public int InvoiceId { get; set; }

        // Stores the invoice number displayed in the report.
        public string InvoiceNumber { get; set; } = string.Empty;

        // Stores the date on which the invoice was issued.
        public DateTime InvoiceDate { get; set; }

        // Stores the name of the customer associated with the invoice.
        public string CustomerName { get; set; } = string.Empty;

        // Stores the current status of the invoice.
        public string Status { get; set; } = string.Empty;

        // Stores the invoice amount before additional totals such as VAT.
        public decimal Subtotal { get; set; }

        // Stores the final invoice total.
        public decimal Total { get; set; }

        // Stores the amount already paid towards the invoice.
        public decimal AmountPaid { get; set; }

        // Stores the remaining amount owed on the invoice.
        public decimal OutstandingBalance { get; set; }
    }
}