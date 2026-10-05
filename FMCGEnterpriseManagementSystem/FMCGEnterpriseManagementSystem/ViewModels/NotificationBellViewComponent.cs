// Title: View Components in ASP.NET Core
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.ViewComponents
{
    // View component responsible for displaying the notification
    // bell and the number of unread notifications.
    //
    // View components are reusable UI components that can contain
    // their own logic and can be invoked from Razor views.
    public class NotificationBellViewComponent : ViewComponent
    {
        // Service used to retrieve notification information.
        // The dependency is provided through constructor injection.
        private readonly INotificationService _notificationService;

        // Constructor receives the notification service through
        // ASP.NET Core dependency injection.
        public NotificationBellViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Executes the view component asynchronously.
        // It retrieves the number of unread notifications and
        // passes that value to the component's Razor view.
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieves the current unread notification count.
            var count = await _notificationService.GetUnreadCountAsync();

            // Renders the view component using the unread count
            // as the model for the associated view.
            return View(count);
        }
    }
}

