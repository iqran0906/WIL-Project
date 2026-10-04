

/***************************************************************************************
*    Title: Enumeration types - C# reference
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.Enums
{
    // Defines the possible stages of an invoice throughout its lifecycle.
    public enum InvoiceStatus
    {
        Draft,
        Pending,
        Approved,
        PartiallyPaid,
        Paid,
        Overdue,
        Cancelled
    }
}