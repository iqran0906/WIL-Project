// Purpose: Contract (interface) for the activity service.
// Authors: ST10068525 (new file, not yet committed)

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Records and reads the Recent Activity log
    public interface IActivityService
    {
        Task LogAsync(string userId, string? userName, string category, string description, string? details = null);

        // Newest first. userId = null returns everyone's activity (administrators only).
        Task<IReadOnlyList<ActivityLog>> GetRecentAsync(string? userId, int take = 100);
    }
}
