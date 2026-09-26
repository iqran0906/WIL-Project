using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetAllAsync();
            return View(notifications);
        }

        // GET: Notifications/Unread (for a bell icon dropdown, or a filtered view)
        public async Task<IActionResult> Unread()
        {
            var notifications = await _notificationService.GetUnreadAsync();
            return View("Index", notifications);
        }

        // POST: Notifications/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}