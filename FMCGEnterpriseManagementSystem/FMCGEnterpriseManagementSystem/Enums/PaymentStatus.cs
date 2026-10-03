/***************************************************************************************
*    Title: Payment Status Enumeration
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Enums/PaymentStatus.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Enumeration types - C# reference
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.Enums
{
    // Defines the payment states used to track outstanding and completed payments.
    public enum PaymentStatus
    {
        Unpaid,
        PartiallyPaid,
        Paid,
        Overdue
    }
}