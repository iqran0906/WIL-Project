// Purpose: Dashboard (with charts), customer list, user profile, error pages and older prototype pages.
// Authors: Sayali-St10458649, iqran0906, ST10068525, Naseeha27 (from git history)
// Uses: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

using System.Diagnostics;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();
        // Old link - settings now live in SettingsController
        public IActionResult Settings() => RedirectToAction("Index", "Settings");
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            var salesRep = employee == null
                ? null
                : await _context.SalesRepresentatives
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sr => sr.EmployeeID == employee.EmployeeID);

            var model = new UserProfileViewModel
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),

                HasEmployeeRecord = employee != null,
                FirstName = employee?.FirstName,
                LastName = employee?.LastName,
                EmployeeNumber = employee?.EmployeeNumber,
                ContactNumber = employee?.ContactNumber ?? user.PhoneNumber,
                JobTitle = employee?.JobTitle,
                DateOfEmployment = employee?.DateOfEmployment,

                SalesRepCode = salesRep?.SalesRepCode,
                SalesArea = salesRep?.Area
            };

            return View(model);
        }
        public IActionResult Privacy() => View();
        // GET: Home/Dashboard?months=12
        public async Task<IActionResult> Dashboard(int months = 12)
        {
            if (months is not (6 or 12 or 24))
            {
                months = 12;
            }

            var today = DateTime.Today;
            var firstMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-(months - 1));

            // Income = value invoiced (incl. VAT) per calendar month
            var invoiced = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.InvoiceDate >= firstMonth)
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(i => i.Total) })
                .ToListAsync();

            // Every month in the range, including months with no sales
            var incomeByMonth = Enumerable.Range(0, months)
                .Select(offset => firstMonth.AddMonths(offset))
                .Select(month => new MonthlyIncome
                {
                    Year = month.Year,
                    Month = month.Month,
                    Label = month.ToString("MMM yyyy"),
                    Income = invoiced
                        .FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month)?.Total ?? 0
                })
                .ToList();

            // Sales per product category over the same period
            var salesByCategory = await _context.InvoiceItems
                .AsNoTracking()
                .Where(ii => ii.Invoice.InvoiceDate >= firstMonth)
                .GroupBy(ii => ii.Product.Category)
                .Select(g => new CategorySales
                {
                    Category = g.Key,
                    Sales = g.Sum(ii => ii.LineTotal)
                })
                .Where(c => c.Sales > 0)
                .OrderByDescending(c => c.Sales)
                .ToListAsync();

            foreach (var item in salesByCategory.Where(c => string.IsNullOrWhiteSpace(c.Category)))
            {
                item.Category = "Uncategorised";
            }

            return View(new DashboardViewModel
            {
                Months = months,
                IncomeByMonth = incomeByMonth,
                SalesByCategory = salesByCategory
            });
        }
        public IActionResult Reports() => View();

        [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
        public async Task<IActionResult> CustomerList(string? keyword)
        {
            var customers = _context.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim();
                var isNumber = int.TryParse(term, out var customerNumber);

                customers = customers.Where(c =>
                    (isNumber && c.CustomerId == customerNumber) ||
                    (c.Name + " " + c.Surname).Contains(term) ||
                    c.Email.Contains(term) ||
                    c.CellNumber.Contains(term) ||
                    c.TelephoneNumber.Contains(term) ||
                    c.IdNumber.Contains(term) ||
                    c.CustomerGroup.Contains(term) ||
                    c.PaymentTerms.Contains(term) ||
                    (c.VATNumber != null && c.VATNumber.Contains(term)));
            }

            var results = await customers
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .ToListAsync();

            // Amount due per customer = invoiced total - payments received
            var invoiced = await _context.Invoices
                .GroupBy(i => i.CustomerId)
                .Select(g => new { CustomerId = g.Key, Total = g.Sum(i => i.Total) })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Total);

            var paid = await _context.Payments
                .GroupBy(p => p.Invoice.CustomerId)
                .Select(g => new { CustomerId = g.Key, Total = g.Sum(p => p.AmountPaid) })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Total);

            ViewBag.AmountDue = results.ToDictionary(
                c => c.CustomerId,
                c => invoiced.GetValueOrDefault(c.CustomerId) - paid.GetValueOrDefault(c.CustomerId));

            ViewBag.Keyword = keyword;

            return View(results);
        }
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

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError(
                    exceptionFeature.Error,
                    "Unhandled error on {Path} (request {RequestId})",
                    exceptionFeature.Path,
                    requestId);
            }

            return View(new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = 500
            });
        }

        // Shown for "not found", "bad request", etc. (see UseStatusCodePagesWithReExecute)
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusCodePage(int code)
        {
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            _logger.LogWarning(
                "Status code {StatusCode} returned for {Path}",
                code,
                reExecute?.OriginalPath ?? "(unknown)");

            Response.StatusCode = code;

            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code
            });
        }
    }
}