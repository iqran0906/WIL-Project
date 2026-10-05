// Title: Decimal numeric types - C# reference
// Author: Microsoft
// Date: 29-09-2022
// Code version: C# 14 / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
//
namespace FMCGEnterpriseManagementSystem.Models
{
    // Stores VAT rate settings and their effective dates.
    public class VatSettings
    {
        // Primary key that uniquely identifies the VAT settings record.
        public int Id { get; set; }

        // Stores the VAT rate as a percentage.
        // The default value represents the South African standard VAT rate.
        public decimal VatRate { get; set; } = 15.00m;

        // Indicates whether this VAT setting is currently active.
        public bool IsActive { get; set; } = true;

        // Records the date from which the VAT rate becomes effective.
        public DateTime EffectiveFrom { get; set; }
    }
}