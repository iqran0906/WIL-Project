/***************************************************************************************
*    Title: Notification Data Transfer Object
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/DTOs/NotificationDto.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Classes and Objects - C# Programming Guide
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.DTOs
{
    // Stores notification information passed between the notification services and observers.
    public class NotificationDto
    {
        // Identifies the type of notification being created.
        public NotificationType Type { get; set; }

        // Stores the notification heading displayed to the user.
        public string Title { get; set; }

        // Stores the main notification message.
        public string Message { get; set; }

        // Optionally identifies the database record related to the notification.
        public int? RelatedEntityId { get; set; }

        // Identifies the type of entity associated with the notification.
        public string RelatedEntityType { get; set; }
    }
}