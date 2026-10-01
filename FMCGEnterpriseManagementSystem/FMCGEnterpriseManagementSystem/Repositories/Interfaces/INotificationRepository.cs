// Purpose: Contract (interface) for notification data access.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification notification);
        Task<Notification> GetByIdAsync(int id);
        Task<List<Notification>> GetAllAsync();
        Task<List<Notification>> GetUnreadAsync();

        Task<int> GetUnreadCountAsync();
        Task<List<Notification>> GetByTypeAsync(NotificationType type);
        Task MarkAsReadAsync(int id);
        Task MarkEmailSentAsync(int id);
    }
}