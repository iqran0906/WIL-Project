using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Factories
{
    public static class NotificationFactory
    {
        public static Notification Create(NotificationDto dto)
        {
            return new Notification
            {
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                RelatedEntityId = dto.RelatedEntityId,
                RelatedEntityType = dto.RelatedEntityType,
                IsRead = false,
                EmailSent = true // every notification type currently requires an email
            };
        }
    }
}