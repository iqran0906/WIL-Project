using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Title: Controller for displaying sales and inventory analytics.
    // Authors: Microsoft
    // Date: 27-04-2026
    // Code version: ASP.NET Core 10.0
    // Availability: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

    // Restricts access to authenticated users.
    [Authorize]
    public class AnalyticsController : Controller
    {
        // Service used to retrieve sales and inventory analytics data.
        private readonly IAnalyticsService _analyticsService;

        // Dependency injection provides the analytics service to the controller.
        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // Retrieves the sales and inventory trends for the analytics page.
        public async Task<IActionResult> Index()
        {
            // Requests the required analytics data from the analytics service.
            var model = await _analyticsService.GetSalesAndInventoryTrendsAsync();

            // Sends the analytics model to the associated View.
            return View(model);
        }
    }
}