// Title: ASP.NET Core MVC View Models
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // View model used to transfer payment information to payment reports.
    // It contains only the fields required for displaying report results.
    public class PaymentReportViewModel
    {
        // Unique identifier of the payment.
        public int PaymentId { get; set; }

        // Date on which the payment was recorded.
        public DateTime PaymentDate { get; set; }

        // Invoice number associated with the payment.
        public string InvoiceNumber { get; set; } = string.Empty;

        // Name of the customer who made the payment.
        public string CustomerName { get; set; } = string.Empty;

        // Method used to make the payment, such as Cash, Card or EFT.
        public string PaymentMethod { get; set; } = string.Empty;

        // Monetary amount received for the payment.
        public decimal AmountPaid { get; set; }
    }
}
