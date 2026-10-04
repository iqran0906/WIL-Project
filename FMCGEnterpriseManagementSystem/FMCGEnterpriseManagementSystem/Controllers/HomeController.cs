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

// Title: Controller for handling home, dashboard, profile, legacy routes and application errors.
// Authors: ImranHussain78612
// Date: 27-04-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Requires users to be authenticated to access the Home Controller.
    [Authorize]
    public class HomeController : Controller
    {
        // Logger used to record application errors and warnings.
        private readonly ILogger<HomeController> _logger;

        // Database context used to retrieve application data.
        private readonly ApplicationDbContext _context;

        // ASP.NET Core Identity manager used to retrieve the current user and their roles.
        private readonly UserManager<User> _userManager;

        // Service responsible for retrieving dashboard analytics.
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
            // Retrieves the dashboard analytics from the dashboard service.
            var analytics =
                await _dashboardService.GetDashboardAnalyticsAsync();

            // Displays the Dashboard View with the retrieved analytics.
            return View("Dashboard", analytics);
        }

        // Direct Dashboard route with an optional number of months for the analytics.
        [HttpGet]
        public async Task<IActionResult> Dashboard(int months = 12)
        {
            // Retrieves dashboard analytics for the requested number of months.
            var analytics =
                await _dashboardService.GetDashboardAnalyticsAsync(months);

            // Displays the Dashboard View using the retrieved analytics.
            return View("Dashboard", analytics);
        }

        // Redirects Settings requests to the dedicated Settings controller.
        [HttpGet]
        public IActionResult Settings()
        {
            // Sends the user to the Settings controller's Index action.
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
                    // Stores the user's username.
                    UserName =
                        user.UserName ?? string.Empty,

                    // Stores the user's email address.
                    Email =
                        user.Email ?? string.Empty,

                    // Stores the user's active status.
                    IsActive =
                        user.IsActive,

                    // Retrieves all roles assigned to the current user.
                    Roles =
                        (await _userManager
                            .GetRolesAsync(user))
                        .ToList(),

                    // Indicates whether the user has a linked employee record.
                    HasEmployeeRecord =
                        employee != null,

                    // Stores the employee's first name when available.
                    FirstName =
                        employee?.FirstName,

                    // Stores the employee's last name when available.
                    LastName =
                        employee?.LastName,

                    // Stores the employee number when available.
                    EmployeeNumber =
                        employee?.EmployeeNumber,

                    // Uses the employee contact number or the user's phone number.
                    ContactNumber =
                        employee?.ContactNumber ??
                        user.PhoneNumber,

                    // Stores the employee's job title when available.
                    JobTitle =
                        employee?.JobTitle,

                    // Stores the employee's date of employment when available.
                    DateOfEmployment =
                        employee?.DateOfEmployment,

                    // Stores the sales representative code when available.
                    SalesRepCode =
                        salesRep?.SalesRepCode,

                    // Stores the sales representative's assigned area.
                    SalesArea =
                        salesRep?.Area
                };

            // Sends the completed profile model to the View.
            return View(model);
        }

        // Displays the privacy page.
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        // Redirects Reports requests to the dedicated Reports controller.
        [HttpGet]
        public IActionResult Reports()
        {
            // Sends the user to the Reports controller's Index action.
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
            // Redirects the request to the Customers controller.
            return RedirectToAction(
                "CustomerList",
                "Customers");
        }

        // Legacy routes retained so older prototype links do not break.
        [HttpGet]
        public IActionResult SupplierList()
        {
            // Redirects the request to the Supplier controller.
            return RedirectToAction(
                "SupplierList",
                "Supplier");
        }

        [HttpGet]
        public IActionResult EmployeeList()
        {
            // Redirects the request to the Employees controller.
            return RedirectToAction(
                "Index",
                "Employees");
        }

        [HttpGet]
        public IActionResult InventoryList()
        {
            // Redirects the request to the Inventory controller.
            return RedirectToAction(
                "Index",
                "Inventory");
        }

        [HttpGet]
        public IActionResult InvoiceList()
        {
            // Redirects the request to the Invoices controller.
            return RedirectToAction(
                "Index",
                "Invoices");
        }

        [HttpGet]
        public IActionResult QuoteList()
        {
            // Redirects the request to the Quotes controller.
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

            // Retrieves information about the exception that caused the error.
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

            // Creates the error model and displays the error View.
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
            // Retrieves information about the original request that produced the status code.
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

            // Sets the HTTP response status code.
            Response.StatusCode = code;

            // Displays the error View with the relevant status code.
            return View(
                "Error",
                new ErrorViewModel
                {
                    // Creates a request identifier for tracing the error.
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier,

                    // Stores the HTTP status code in the error model.
                    StatusCode = code
                });
        }
    }
}