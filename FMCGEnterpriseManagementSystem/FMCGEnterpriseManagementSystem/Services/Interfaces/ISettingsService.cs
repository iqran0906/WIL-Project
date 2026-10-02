// Purpose: Contract (interface) for the settings service.
// Authors: ST10068525 (new file, not yet committed)

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
