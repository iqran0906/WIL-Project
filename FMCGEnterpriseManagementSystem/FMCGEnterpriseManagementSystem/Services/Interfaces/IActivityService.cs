// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to record and retrieve recent system activity.
    public interface IActivityService
    {
        // Records an activity performed by a user.
        // The activity includes the user, category, description and optional details.
        Task LogAsync(string userId, string? userName, string category, string description, string? details = null);

        // Retrieves recent activity records, with the newest records returned first.
        // When userId is null, activity for all users can be retrieved.
        // The take parameter limits the number of activity records returned.
        Task<IReadOnlyList<ActivityLog>> GetRecentAsync(string? userId, int take = 100);
    }
}