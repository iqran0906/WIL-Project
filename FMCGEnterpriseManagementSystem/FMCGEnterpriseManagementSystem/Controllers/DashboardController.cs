using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var analytics = await _dashboardService.GetDashboardAnalyticsAsync();
            return View("~/Views/Dashboard/Index.cshtml", analytics);
        }
    }
}