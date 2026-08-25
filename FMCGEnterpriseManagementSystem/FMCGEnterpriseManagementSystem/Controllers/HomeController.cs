using System.Diagnostics;
using FMCGEnterpriseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();
        public IActionResult Login() => View();
        public IActionResult Settings() => View();
        public IActionResult UserProfile() => View();
        public IActionResult Privacy() => View();
        public IActionResult Dashboard() => View();
        public IActionResult Reports() => View();
        public IActionResult Notifications() => View();

        public IActionResult CustomerList() => View();
        public IActionResult SupplierList() => View();
        public IActionResult EmployeeList() => View();
        public IActionResult InventoryList() => View();
        public IActionResult InvoiceList() => View();
        public IActionResult QuoteList() => View();

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