using FMCGEnterpriseManagementSystem.DTOs;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    public interface INotificationObserver
    {
        Task HandleAsync(NotificationDto notificationDto);
    }
}