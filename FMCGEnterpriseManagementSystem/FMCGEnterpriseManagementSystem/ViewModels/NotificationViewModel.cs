// Title: ASP.NET Core MVC Views and ViewModels
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model containing notification information
    // that can be displayed in the application's user interface.
    public class NotificationViewModel
    {
        // Unique identifier of the notification.
        public int NotificationId { get; set; }

        // User-friendly representation of the notification type.
        public string TypeDisplay { get; set; }

        // Short title displayed for the notification.
        public string Title { get; set; }

        // Main notification message presented to the user.
        public string Message { get; set; }

        // Indicates whether the notification has already been read.
        public bool IsRead { get; set; }

        // Indicates whether the notification has been sent by email.
        public bool EmailSent { get; set; }

        // Formatted date and time used when displaying
        // when the notification was created.
        public string CreatedDateDisplay { get; set; }

        // Optional identifier of the business entity associated
        // with the notification.
        public int? RelatedEntityId { get; set; }

        // Identifies the type of business entity related
        // to the notification.
        public string RelatedEntityType { get; set; }
    }
}

