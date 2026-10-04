

/***************************************************************************************
*    Title: Enumeration types - C# reference
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.Enums
{
    // Defines the stages a customer quote can have before it is completed or cancelled.
    public enum QuoteStatus
    {
        Pending,
        Invoiced,
        Cancelled
    }
}