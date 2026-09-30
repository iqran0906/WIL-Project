// Purpose: Saves and reads Recent Activity entries.
// Authors: ST10068525 (new file, not yet committed)

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Saves and loads Recent Activity entries
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ApplicationDbContext context, ILogger<ActivityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(string userId, string? userName, string category, string description, string? details = null)
        {
            var entry = new ActivityLog
            {
                UserId = userId,
                UserName = userName,
                Category = Truncate(category, 50)!,
                Description = Truncate(description, 200)!,
                Details = Truncate(details, 500),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                _context.ActivityLogs.Add(entry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // The activity log must never break the action the user just did
                _context.Entry(entry).State = EntityState.Detached;
                _logger.LogWarning(ex, "Could not record activity '{Description}' for {User}", description, userName);
            }
        }

        public async Task<IReadOnlyList<ActivityLog>> GetRecentAsync(string? userId, int take = 100)
        {
            var query = _context.ActivityLogs.AsNoTracking();

            if (userId != null)
            {
                query = query.Where(a => a.UserId == userId);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ThenByDescending(a => a.ActivityLogId)
                .Take(take)
                .ToListAsync();
        }

        private static string? Truncate(string? value, int max) =>
            value == null || value.Length <= max ? value : value[..max];
    }
}
