/***************************************************************************************
*    Title: Analytics Controller
*    Author:Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Controllers/AnalyticsController.cs
***************************************************************************************/

using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
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