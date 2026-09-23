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
        Task<List<Notification>> GetByTypeAsync(NotificationType type);
        Task MarkAsReadAsync(int id);
        Task MarkEmailSentAsync(int id);
    }
}