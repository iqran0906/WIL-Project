// Purpose: Recent Activity page (clock icon in the top bar).
// Authors: ST10068525 (new file, not yet committed)

using System.Security.Claims;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Recent Activity page (clock icon in the top bar).
    // Everyone sees their own activity; administrators can also view everyone's.
    [Authorize]
    public class ActivityController : Controller
    {
        private const int MaxItems = 100;

        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // GET: Activity?scope=all
        [HttpGet]
        public async Task<IActionResult> Index(string? scope)
        {
            var showEveryone = scope == "all" && User.IsInRole("Administrator");

            var userId = showEveryone
                ? null
                : User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activities = await _activityService.GetRecentAsync(userId, MaxItems);

            ViewBag.ShowEveryone = showEveryone;
            ViewBag.CanViewEveryone = User.IsInRole("Administrator");
            ViewBag.MaxItems = MaxItems;

            return View(activities);
        }
    }
}
