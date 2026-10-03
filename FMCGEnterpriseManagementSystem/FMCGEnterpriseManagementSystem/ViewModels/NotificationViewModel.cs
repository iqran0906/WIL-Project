/***************************************************************************************
*    Title: Notification View Model
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/NotificationViewModel.cs
***************************************************************************************/

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