// Purpose: Data and validation rules for the notification pages and forms.
// Authors: Sayali-St10458649 (from git history)

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string TypeDisplay { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public bool EmailSent { get; set; }
        public string CreatedDateDisplay { get; set; }
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
    }
}