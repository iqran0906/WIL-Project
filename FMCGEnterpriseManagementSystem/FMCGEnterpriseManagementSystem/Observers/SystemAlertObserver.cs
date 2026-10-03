/***************************************************************************************
*    Title: In-App Notification Observer
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Observers/InAppNotificationObserver.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Dependency injection in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
***************************************************************************************/

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    public class SystemAlertObserver : INotificationObserver
    {
        private readonly INotificationRepository _notificationRepository;

        public SystemAlertObserver(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task HandleAsync(NotificationDto notificationDto)
        {
            var notification = NotificationFactory.Create(notificationDto);
            await _notificationRepository.AddAsync(notification);
        }
    }
}