// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for saving and retrieving recent activity records.
    public class ActivityService : IActivityService
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Used to record warnings when activity logging fails.
        private readonly ILogger<ActivityService> _logger;

        // Initialises the service with the database context and logger.
        public ActivityService(ApplicationDbContext context, ILogger<ActivityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Records an activity performed by a user.
        // The activity information is truncated to prevent excessively long database values.
        public async Task LogAsync(string userId, string? userName, string category, string description, string? details = null)
        {
            // Creates a new activity log entry using the supplied user and activity information.
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
                // Adds the activity entry to the database context.
                _context.ActivityLogs.Add(entry);

                // Saves the new activity record asynchronously.
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // The activity log must never break the action the user just did.
                // Detaches the failed entry so it does not remain tracked by the context.
                _context.Entry(entry).State = EntityState.Detached;

                // Records a warning containing details about the failed activity log.
                _logger.LogWarning(ex, "Could not record activity '{Description}' for {User}", description, userName);
            }
        }

        // Retrieves the most recent activity records.
        // When a user ID is supplied, only activities belonging to that user are returned.
        public async Task<IReadOnlyList<ActivityLog>> GetRecentAsync(string? userId, int take = 100)
        {
            // Creates a query that does not track the returned entities because they are read-only.
            var query = _context.ActivityLogs.AsNoTracking();

            // Filters the results to a specific user when a user ID is provided.
            if (userId != null)
            {
                query = query.Where(a => a.UserId == userId);
            }

            // Sorts activities from newest to oldest, limits the number of results,
            // and executes the query asynchronously.
            return await query
                .OrderByDescending(a => a.Timestamp)
                .ThenByDescending(a => a.ActivityLogId)
                .Take(take)
                .ToListAsync();
        }

        // Limits a string to the specified maximum length.
        // Returns the original value when it is null or already within the limit.
        private static string? Truncate(string? value, int max) =>
            value == null || value.Length <= max ? value : value[..max];
    }
}