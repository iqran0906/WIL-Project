// Purpose: Helper for VAT calculations.
// Authors: Sayali-St10458649 (from git history)

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