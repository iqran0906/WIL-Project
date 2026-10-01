// Purpose: Reads (cached) and saves the company-wide settings.
// Authors: ST10068525 (new file, not yet committed)

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class SettingsService : ISettingsService
    {
        private const string CacheKey = "SystemSettings";

        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public SettingsService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<SystemSetting> GetAsync()
        {
            if (_cache.TryGetValue(CacheKey, out SystemSetting? cached) && cached != null)
            {
                return cached;
            }

            var settings = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == SystemSetting.SingletonId);

            if (settings == null)
            {
                // First run: store the defaults so admins can edit them
                settings = new SystemSetting();
                _context.SystemSettings.Add(settings);
                await _context.SaveChangesAsync();
                _context.Entry(settings).State = EntityState.Detached;
            }

            _cache.Set(CacheKey, settings, TimeSpan.FromMinutes(30));

            return settings;
        }

        public async Task<decimal> GetVatRateAsync()
        {
            var settings = await GetAsync();
            return settings.VatRatePercent / 100m;
        }

        public async Task UpdateAsync(SystemSetting settings, string? updatedBy)
        {
            // Make sure the row exists
            await GetAsync();

            settings.Id = SystemSetting.SingletonId;
            settings.UpdatedAt = DateTime.UtcNow;
            settings.UpdatedBy = updatedBy;

            _context.SystemSettings.Update(settings);
            await _context.SaveChangesAsync();
            _context.Entry(settings).State = EntityState.Detached;

            _cache.Remove(CacheKey);
        }
    }
}
