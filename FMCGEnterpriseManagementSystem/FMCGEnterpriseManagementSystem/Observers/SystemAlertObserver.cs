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