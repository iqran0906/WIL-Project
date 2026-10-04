

/***************************************************************************************
*    Title: Dependency injection in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
***************************************************************************************/

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Factories
{
    // Creates Notification objects from the notification data received by the application.
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