// Purpose: Observer pattern: contract for anything that reacts to a notification.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.DTOs;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    public interface INotificationObserver
    {
        Task HandleAsync(NotificationDto notificationDto);
    }
}