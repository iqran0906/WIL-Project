using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _analyticsService.GetSalesAndInventoryTrendsAsync();
            return View(model);
        }
    }
}