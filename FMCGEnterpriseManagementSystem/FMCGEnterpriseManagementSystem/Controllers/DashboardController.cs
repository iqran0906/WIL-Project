// Title: Controller for displaying dashboard analytics.
// Authors: Maseeha17
// Date: 27-04-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts access to authenticated users.
    [Authorize]
    public class DashboardController : Controller
    {
        // Service responsible for retrieving dashboard analytics.
        private readonly IDashboardService _dashboardService;

        // Dependency injection provides the dashboard service to the controller.
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // Retrieves the dashboard analytics and displays the dashboard.
        public async Task<IActionResult> Index()
        {
            // Fetch live database calculations.
            var model = await _dashboardService.GetDashboardAnalyticsAsync();

            // Sends the dashboard data to the Dashboard View.
            return View("~/Views/Home/Dashboard.cshtml", model);
        }
    }
}