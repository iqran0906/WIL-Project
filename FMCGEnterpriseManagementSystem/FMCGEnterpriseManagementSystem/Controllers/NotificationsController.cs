
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only Administrators, Employees and Sales Representatives can access notifications.
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        // Dependency injection provides the notification service.
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: Notifications
        // Retrieves and displays all notifications.
        public async Task<IActionResult> Index()
        {
            var notifications =
                await _notificationService.GetAllAsync();

            return View(notifications);
        }

        // GET: Notifications/Unread
        // Retrieves only unread notifications for the notification dropdown or view.
        public async Task<IActionResult> Unread()
        {
            var notifications =
                await _notificationService.GetUnreadAsync();

            return View("Index", notifications);
        }

        // POST: Notifications/MarkAsRead/5
        // Marks the selected notification as read.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}