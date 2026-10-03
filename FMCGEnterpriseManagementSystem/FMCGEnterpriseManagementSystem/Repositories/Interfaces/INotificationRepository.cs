// P/***************************************************************************************
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

*Title: Notification Repository Interface
* Author: Sayali - St10458649
* Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem / Repositories / Interfaces / INotificationRepository.cs
* **************************************************************************************/
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