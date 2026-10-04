
//    Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 08-04-2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // ViewModel used to transfer accounts receivable ageing information
    // from the reporting layer to the report view.
    public class AgeAnalysisReportViewModel
    {
        // Stores the unique invoice number displayed in the report.
        public string InvoiceNumber { get; set; } = string.Empty;

        // Stores the name of the customer associated with the invoice.
        public string CustomerName { get; set; } = string.Empty;

        // Stores the date on which the invoice was issued.
        public DateTime InvoiceDate { get; set; }

        // Stores the number of days the invoice has been outstanding.
        public int AgeInDays { get; set; }

        // Stores the original total amount of the invoice.
        public decimal InvoiceTotal { get; set; }

        // Stores the amount that has already been paid towards the invoice.
        public decimal AmountPaid { get; set; }

        // Stores the remaining amount that is still owed by the customer.
        public decimal OutstandingBalance { get; set; }

        // Stores the ageing category assigned to the outstanding invoice.
        // Examples include 0-30 Days, 31-60 Days and 90+ Days.
        public string AgeBracket { get; set; } = string.Empty;
    }
}