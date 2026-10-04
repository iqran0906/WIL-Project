// Title: Dependency injection in ASP.NET Core
// Author: Microsoft
// Date: 18-09-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    // Observer responsible for storing notifications in the system.
    public class SystemAlertObserver : INotificationObserver
    {
        // Repository used to save notifications to the database.
        private readonly INotificationRepository _notificationRepository;

        // Injects the notification repository through dependency injection.
        public SystemAlertObserver(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        // Converts the DTO into a notification entity and stores it.
        public async Task HandleAsync(NotificationDto notificationDto)
        {
            // Uses the factory to create the appropriate notification entity.
            var notification = NotificationFactory.Create(notificationDto);

            // Saves the notification through the repository.
            await _notificationRepository.AddAsync(notification);
        }
    }
}