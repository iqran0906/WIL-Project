// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Provides access to the application's system-wide settings.
    public class SettingsService : ISettingsService
    {
        // Key used to store and retrieve the settings from memory cache.
        private const string CacheKey = "SystemSettings";

        // Provides access to the database.
        private readonly ApplicationDbContext _context;

        // Provides in-memory caching functionality.
        private readonly IMemoryCache _cache;

        // Constructor receives dependencies through dependency injection.
        public SettingsService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // Retrieves the system settings.
        public async Task<SystemSetting> GetAsync()
        {
            // Checks whether the settings have already been stored in memory.
            // This avoids unnecessary database queries.
            if (_cache.TryGetValue(CacheKey, out SystemSetting? cached) && cached != null)
            {
                return cached;
            }

            // Retrieves the single system settings record from the database.
            var settings = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == SystemSetting.SingletonId);

            // If no settings record exists, create the default settings.
            if (settings == null)
            {
                // Creates a new settings object using the model's defaults.
                settings = new SystemSetting();

                // Adds the default settings to the database.
                _context.SystemSettings.Add(settings);
                await _context.SaveChangesAsync();

                // Detaches the entity because it is no longer being modified
                // by this database context.
                _context.Entry(settings).State = EntityState.Detached;
            }

            // Stores the settings in memory for 30 minutes.
            _cache.Set(CacheKey, settings, TimeSpan.FromMinutes(30));

            return settings;
        }

        // Retrieves the VAT rate as a decimal value.
        public async Task<decimal> GetVatRateAsync()
        {
            // Gets the current system settings.
            var settings = await GetAsync();

            // Converts the stored percentage into a decimal rate.
            return settings.VatRatePercent / 100m;
        }

        // Updates the system settings.
        public async Task UpdateAsync(SystemSetting settings, string? updatedBy)
        {
            // Ensures that the settings row exists before updating it.
            await GetAsync();

            // Ensures that the correct singleton record ID is used.
            settings.Id = SystemSetting.SingletonId;

            // Records when the settings were changed.
            settings.UpdatedAt = DateTime.UtcNow;

            // Records which user performed the update.
            settings.UpdatedBy = updatedBy;

            // Marks the settings entity as modified.
            _context.SystemSettings.Update(settings);

            // Saves the changes to the database.
            await _context.SaveChangesAsync();

            // Detaches the updated object from the database context.
            _context.Entry(settings).State = EntityState.Detached;

            // Removes the old cached settings so that the next request
            // retrieves the updated values.
            _cache.Remove(CacheKey);
        }
    }
}