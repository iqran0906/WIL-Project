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