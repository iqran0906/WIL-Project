// Purpose: Dashboard (with charts), customer list, user profile, error pages and older prototype pages.
// Authors: Sayali-St10458649, iqran0906, ST10068525, Naseeha27 (from git history)
// Uses: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

// Purpose: Dashboard, user profile, error pages and legacy route support.

using System.Diagnostics;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IDashboardService _dashboardService;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<User> userManager,
            IDashboardService dashboardService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _dashboardService = dashboardService;
        }

        // Project launch route - loads the integrated dashboard.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var analytics =
                await _dashboardService.GetDashboardAnalyticsAsync();

            return View("Dashboard", analytics);
        }

        // Direct Dashboard route.
        [HttpGet]
        public async Task<IActionResult> Dashboard(int months = 12)
        {
            var analytics =
                await _dashboardService.GetDashboardAnalyticsAsync(months);

            return View("Dashboard", analytics);
        }

        // Settings now has its own controller.
        [HttpGet]
        public IActionResult Settings()
        {
            return RedirectToAction("Index", "Settings");
        }

        // Displays information for the currently logged-in user.
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var employee =
                await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        e => e.UserId == user.Id);

            var salesRep =
                employee == null
                    ? null
                    : await _context.SalesRepresentatives
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            sr => sr.EmployeeID ==
                                  employee.EmployeeID);

            var model =
                new UserProfileViewModel
                {
                    UserName =
                        user.UserName ?? string.Empty,

                    Email =
                        user.Email ?? string.Empty,

                    IsActive =
                        user.IsActive,

                    Roles =
                        (await _userManager
                            .GetRolesAsync(user))
                        .ToList(),

                    HasEmployeeRecord =
                        employee != null,

                    FirstName =
                        employee?.FirstName,

                    LastName =
                        employee?.LastName,

                    EmployeeNumber =
                        employee?.EmployeeNumber,

                    ContactNumber =
                        employee?.ContactNumber ??
                        user.PhoneNumber,

                    JobTitle =
                        employee?.JobTitle,

                    DateOfEmployment =
                        employee?.DateOfEmployment,

                    SalesRepCode =
                        salesRep?.SalesRepCode,

                    SalesArea =
                        salesRep?.Area
                };

            return View(model);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        // Reports have their own controller.
        [HttpGet]
        public IActionResult Reports()
        {
            return RedirectToAction(
                "Index",
                "Reports");
        }

        // Legacy customer route - redirect to the real Customers feature.
        [HttpGet]
        [Authorize(
            Roles =
                "Administrator,Employee,SalesRepresentative")]
        public IActionResult CustomerList()
        {
            return RedirectToAction(
                "CustomerList",
                "Customers");
        }

        // Legacy routes retained so older prototype links do not break.
        [HttpGet]
        public IActionResult SupplierList()
        {
            return RedirectToAction(
                "SupplierList",
                "Supplier");
        }

        [HttpGet]
        public IActionResult EmployeeList()
        {
            return RedirectToAction(
                "Index",
                "Employees");
        }

        [HttpGet]
        public IActionResult InventoryList()
        {
            return RedirectToAction(
                "Index",
                "Inventory");
        }

        [HttpGet]
        public IActionResult InvoiceList()
        {
            return RedirectToAction(
                "Index",
                "Invoices");
        }

        [HttpGet]
        public IActionResult QuoteList()
        {
            return RedirectToAction(
                "Index",
                "Quotes");
        }

        // Older prototype pages.
        public IActionResult AddCustomer() => View();

        public IActionResult AddSupplier() => View();

        public IActionResult AddSalesRep() => View();

        public IActionResult AddEmployee() => View();

        public IActionResult AddItem() => View();

        public IActionResult CreateInvoice() => View();

        public IActionResult CreateQuote() => View();

        [AllowAnonymous]
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            var requestId =
                Activity.Current?.Id ??
                HttpContext.TraceIdentifier;

            var exceptionFeature =
                HttpContext.Features
                    .Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError(
                    exceptionFeature.Error,
                    "Unhandled error on {Path} " +
                    "(request {RequestId})",
                    exceptionFeature.Path,
                    requestId);
            }

            return View(
                new ErrorViewModel
                {
                    RequestId = requestId,
                    StatusCode = 500
                });
        }

        [AllowAnonymous]
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult StatusCodePage(int code)
        {
            var reExecute =
                HttpContext.Features
                    .Get<IStatusCodeReExecuteFeature>();

            _logger.LogWarning(
                "Status code {StatusCode} " +
                "returned for {Path}",
                code,
                reExecute?.OriginalPath ??
                "(unknown)");

            Response.StatusCode = code;

            return View(
                "Error",
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier,

                    StatusCode = code
                });
        }
    }
}