using System.Diagnostics;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboardService;

        public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        // Project launch route - loads Dashboard view with analytics model
        public async Task<IActionResult> Index()
        {
            var analytics = await _dashboardService.GetDashboardAnalyticsAsync();
            return View("Dashboard", analytics);
        }

        // Direct Dashboard route - loads Dashboard view with analytics model
        public async Task<IActionResult> Dashboard()
        {
            var analytics = await _dashboardService.GetDashboardAnalyticsAsync();
            return View(analytics);
        }

        // Core Page Views
        public IActionResult Settings() => View();
        public IActionResult UserProfile() => View();
        public IActionResult Privacy() => View();
        public IActionResult Reports() => View();

        // Listing Views
        public IActionResult CustomerList() => View();
        public IActionResult SupplierList() => View();
        public IActionResult EmployeeList() => View();
        public IActionResult InventoryList() => View();
        public IActionResult InvoiceList() => View();
        public IActionResult QuoteList() => View();

        // Creation / Operational Views
        public IActionResult AddCustomer() => View();
        public IActionResult AddSupplier() => View();
        public IActionResult AddSalesRep() => View();
        public IActionResult AddEmployee() => View();
        public IActionResult AddItem() => View();
        public IActionResult CreateInvoice() => View();
        public IActionResult CreateQuote() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}