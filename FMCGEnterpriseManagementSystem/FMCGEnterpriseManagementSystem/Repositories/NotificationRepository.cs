// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Purpose: Provides database operations for creating and managing notifications.
    public class NotificationRepository : INotificationRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a new notification and saves the changes.
        public async Task<Notification> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        // Retrieves a notification by its ID.
        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        // Retrieves all notifications ordered from newest to oldest.
        public async Task<List<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // Retrieves unread notifications ordered from newest to oldest.
        public async Task<List<Notification>> GetUnreadAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // Counts the number of unread notifications.
        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }

        // Retrieves notifications matching the specified notification type.
        public async Task<List<Notification>> GetByTypeAsync(NotificationType type)
        {
            return await _context.Notifications
                .Where(n => n.Type == type)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // Finds a notification and marks it as read.
        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        // Finds a notification and marks its email as sent.
        public async Task MarkEmailSentAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.EmailSent = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}