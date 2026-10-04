

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
    // Requires users to be authenticated to access the Home Controller.
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IDashboardService _dashboardService;

        // Dependency injection provides logging, database access,
        // user management and dashboard services.
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

        // Direct Dashboard route with an optional number of months for the analytics.
        [HttpGet]
        public async Task<IActionResult> Dashboard(int months = 12)
        {
            var analytics =
                await _dashboardService.GetDashboardAnalyticsAsync(months);

            return View("Dashboard", analytics);
        }

        // Redirects Settings requests to the dedicated Settings controller.
        [HttpGet]
        public IActionResult Settings()
        {
            return RedirectToAction("Index", "Settings");
        }

        // Displays information for the currently logged-in user.
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            // Retrieves the currently authenticated user.
            var user =
                await _userManager.GetUserAsync(User);

            // Redirects to login if no authenticated user is found.
            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // Retrieves the employee record linked to the user.
            var employee =
                await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        e => e.UserId == user.Id);

            // Retrieves the sales representative record when an employee exists.
            var salesRep =
                employee == null
                    ? null
                    : await _context.SalesRepresentatives
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            sr => sr.EmployeeID ==
                                  employee.EmployeeID);

            // Builds the view model using user, employee and sales representative information.
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

        // Redirects Reports requests to the dedicated Reports controller.
        [HttpGet]
        public IActionResult Reports()
        {
            return RedirectToAction(
                "Index",
                "Reports");
        }

        // Legacy customer route - redirects to the real Customers feature.
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

        // Older prototype pages retained for compatibility.
        public IActionResult AddCustomer() => View();

        public IActionResult AddSupplier() => View();

        public IActionResult AddSalesRep() => View();

        public IActionResult AddEmployee() => View();

        public IActionResult AddItem() => View();

        public IActionResult CreateInvoice() => View();

        public IActionResult CreateQuote() => View();

        // Allows the error page to be displayed without authentication.
        [AllowAnonymous]
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            // Creates an identifier that can be used to trace the request.
            var requestId =
                Activity.Current?.Id ??
                HttpContext.TraceIdentifier;

            var exceptionFeature =
                HttpContext.Features
                    .Get<IExceptionHandlerPathFeature>();

            // Logs the unhandled exception and the request path.
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

        // Displays the error page for HTTP status codes such as 404 or 403.
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

            // Logs the returned status code and original request path.
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