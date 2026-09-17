using System;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public bool EmailSent { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Optional link back to the record that triggered it (invoice, quote, product, etc.)
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
    }
}