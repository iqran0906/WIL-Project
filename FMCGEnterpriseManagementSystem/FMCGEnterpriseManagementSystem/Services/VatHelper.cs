// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

namespace FMCGEnterpriseManagementSystem.Services
{
    // Provides reusable helper methods for calculating VAT-related amounts.
    // This class is static because it does not need to store any instance data.
    public static class VatHelper
    {
        // Calculates the VAT amount based on the supplied amount and VAT percentage.
        public static decimal CalculateVat(decimal amount, decimal vatRate)
        {
            // Converts the percentage into a decimal value,
            // calculates the VAT, and rounds the result to two decimal places.
            return Math.Round(amount * (vatRate / 100m), 2);
        }

        // Calculates the final amount after adding VAT to the original amount.
        public static decimal CalculateTotalWithVat(decimal amount, decimal vatRate)
        {
            // Reuses CalculateVat() to calculate the VAT amount
            // and adds it to the original amount.
            return Math.Round(amount + CalculateVat(amount, vatRate), 2);
        }
    }
}