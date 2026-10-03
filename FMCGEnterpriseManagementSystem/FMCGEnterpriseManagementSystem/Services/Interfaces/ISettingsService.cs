/***************************************************************************************
*    Title: Settings Service Interface
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/ISettingsService.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ISettingsService
    {
        // Current settings (cached; created with defaults the first time)
        Task<SystemSetting> GetAsync();

        // VAT as a fraction, e.g. 15% -> 0.15
        Task<decimal> GetVatRateAsync();

        Task UpdateAsync(SystemSetting settings, string? updatedBy);
    }
}
