/***************************************************************************************
*    Title: VAT Rate Record
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Models/VatRate.cs
***************************************************************************************/
namespace FMCGEnterpriseManagementSystem.Models
{
    public class VatSettings
    {
        public int Id { get; set; }
        public decimal VatRate { get; set; } = 15.00m; // e.g. 15% — South African standard VAT
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveFrom { get; set; }
    }
}