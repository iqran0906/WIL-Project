using System.Security.Claims;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Title: Claims-based authorization in ASP.NET Core
// Author: Microsoft
// Date: 08-04-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/security/authorization/claims
namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Recent Activity page (clock icon in the top bar).
    // Everyone sees their own activity; administrators can also view everyone's.

    // Restricts access to authenticated users.
    [Authorize]
    public class ActivityController : Controller
    {
        // Maximum number of recent activity records displayed.
        private const int MaxItems = 100;

        // Service used to retrieve the user's recent activity.
        private readonly IActivityService _activityService;

        // Dependency injection provides the activity service to the controller.
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // GET: Activity?scope=all
        [HttpGet]
        public async Task<IActionResult> Index(string? scope)
        {
            // Administrators can request all users' activity by using scope=all.
            // Other users can only view their own activity.
            var showEveryone = scope == "all" && User.IsInRole("Administrator");

            // Retrieves the currently authenticated user's ID.
            // A null user ID is used when an administrator is viewing everyone's activity.
            var userId = showEveryone
                ? null
                : User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieves the most recent activity records, limited to MaxItems.
            var activities = await _activityService.GetRecentAsync(userId, MaxItems);

            // Sends information to the View to control what activity information is displayed.
            ViewBag.ShowEveryone = showEveryone;
            ViewBag.CanViewEveryone = User.IsInRole("Administrator");
            ViewBag.MaxItems = MaxItems;

            // Sends the retrieved activity records to the View.
            return View(activities);
        }
    }
}