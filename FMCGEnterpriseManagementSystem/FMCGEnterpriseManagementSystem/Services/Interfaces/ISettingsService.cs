// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the contract for managing system-wide application settings.
    public interface ISettingsService
    {
        // Retrieves the current system settings.
        // Settings are cached and default settings are created when required.
        Task<SystemSetting> GetAsync();

        // Retrieves the current VAT rate as a decimal fraction.
        // For example, a VAT rate of 15% is represented as 0.15.
        Task<decimal> GetVatRateAsync();

        // Updates the system settings and records the user responsible for the update.
        Task UpdateAsync(SystemSetting settings, string? updatedBy);
    }
}