/***************************************************************************************
*    Title: Dashboard Controller
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Type: Source code
*    Availability: FMCGEnterpriseManagementSystem/Controllers/DashboardController.cs
***************************************************************************************/

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