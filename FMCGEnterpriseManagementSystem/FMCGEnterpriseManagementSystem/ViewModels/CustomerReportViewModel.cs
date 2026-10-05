//    Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 08-04-2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // ViewModel used to transfer customer information to customer reports.
    public class CustomerReportViewModel
    {
        // Stores the unique identifier of the customer.
        public int CustomerId { get; set; }

        // Stores the customer's name displayed in the report.
        public string CustomerName { get; set; } = string.Empty;

        // Stores the customer's email address.
        public string Email { get; set; } = string.Empty;

        // Stores the customer's telephone number.
        public string TelephoneNumber { get; set; } = string.Empty;

        // Stores the customer group assigned to the customer.
        public string CustomerGroup { get; set; } = string.Empty;

        // Stores the payment terms associated with the customer.
        public string PaymentTerms { get; set; } = string.Empty;

        // Indicates whether the customer account is currently active.
        public bool IsActive { get; set; }
    }
}