// Purpose: Notification details passed to the notification observers (email / in-app).
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.DTOs
{
    public class NotificationDto
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
    }
}