/***************************************************************************************
*    Title: VAT Calculation Helper
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Helpers/VatHelper.cs
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.Services
{
    public static class VatHelper
    {
        public static decimal CalculateVat(decimal amount, decimal vatRate)
        {
            return Math.Round(amount * (vatRate / 100m), 2);
        }

        public static decimal CalculateTotalWithVat(decimal amount, decimal vatRate)
        {
            return Math.Round(amount + CalculateVat(amount, vatRate), 2);
        }
    }
}