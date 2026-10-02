// Purpose: Observer pattern: saves notifications as in-app alerts.
// Authors: Sayali-St10458649 (from git history)

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