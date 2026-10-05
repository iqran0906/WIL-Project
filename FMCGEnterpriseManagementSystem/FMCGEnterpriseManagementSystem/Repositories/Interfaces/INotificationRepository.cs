// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines database operations for creating, retrieving, and updating notifications.
    public interface INotificationRepository
    {
        // Adds a new notification and returns the created notification.
        Task<Notification> AddAsync(Notification notification);

        // Retrieves a notification by its ID.
        Task<Notification> GetByIdAsync(int id);

        // Retrieves all notifications.
        Task<List<Notification>> GetAllAsync();

        // Retrieves notifications that have not yet been read.
        Task<List<Notification>> GetUnreadAsync();

        // Gets the number of unread notifications.
        Task<int> GetUnreadCountAsync();

        //Mark as read once its opened
        Task MarkAllAsReadAsync();

        // Retrieves notifications by their notification type.
        Task<List<Notification>> GetByTypeAsync(NotificationType type);

        // Marks a notification as read.
        Task MarkAsReadAsync(int id);

        // Marks a notification as having its email sent.
        Task MarkEmailSentAsync(int id);
    }
}