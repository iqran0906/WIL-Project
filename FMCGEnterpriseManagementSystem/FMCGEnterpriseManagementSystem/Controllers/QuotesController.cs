// Purpose: Controller for the quotes pages and form submissions.
// Authors: iqran0906, Naseeha27, Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class QuotesController : Controller
    {
        private readonly IQuoteService _quoteService;
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;

        public QuotesController(
            IQuoteService quoteService,
            ICustomerService customerService,
            ApplicationDbContext context)
        {
            _quoteService = quoteService;
            _customerService = customerService;
            _context = context;
        }

        // GET: Quotes
        public async Task<IActionResult> Index(
            string? period,
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            QuoteStatus? status,
            string? keyword)
        {
            // A period only applies when no dates were picked by hand
            var (periodStart, periodEnd) = GetPeriodRange(period);

            var from = startDate ?? periodStart;
            var to = endDate ?? periodEnd;

            IEnumerable<Quote> quotes;

            if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
            {
                ViewBag.DateError = "The end date cannot be before the start date.";
                quotes = Enumerable.Empty<Quote>();
            }
            else
            {
                quotes = await _quoteService.SearchQuotesAsync(customerId, from, to, status, keyword);
            }

            // Filter options and the values currently applied
            var customers = (await _customerService.GetAllCustomersAsync())
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .Select(c => new
                {
                    c.CustomerId,
                    Display = $"{c.Name} {c.Surname} (No. {c.CustomerId})"
                });

            ViewBag.CustomerFilter = new SelectList(customers, "CustomerId", "Display", customerId);
            ViewBag.StatusFilter = new SelectList(Enum.GetValues<QuoteStatus>(), status);
            ViewBag.Period = period;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Keyword = keyword;

            return View(quotes);
        }

        private static (DateTime? Start, DateTime? End) GetPeriodRange(string? period)
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            return period switch
            {
                "today" => (today, today),
                "week" => (today.AddDays(-(((int)today.DayOfWeek + 6) % 7)), today),
                "month" => (monthStart, today),
                "lastmonth" => (monthStart.AddMonths(-1), monthStart.AddDays(-1)),
                "year" => (new DateTime(today.Year, 1, 1), today),
                _ => (null, null)
            };
        }

        // GET: Quotes/Create
        public async Task<IActionResult> Create()
        {
            var model = new QuoteViewModel
            {
                QuoteDate = DateTime.Today
            };

            await LoadLookupsAsync(model);

            return View(model);
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuoteViewModel model)
        {
            // Rules that need the database
            if (model.CustomerId > 0 &&
                !await _context.Customers.AnyAsync(c => c.CustomerId == model.CustomerId && c.IsActive))
            {
                ModelState.AddModelError(nameof(model.CustomerId), "The selected customer does not exist or is inactive.");
            }

            var productIds = model.Items.Select(i => i.ProductId).Where(id => id > 0).Distinct().ToList();
            var activeIds = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && p.IsActive)
                .Select(p => p.ProductId)
                .ToListAsync();

            for (var i = 0; i < model.Items.Count; i++)
            {
                if (model.Items[i].ProductId > 0 && !activeIds.Contains(model.Items[i].ProductId))
                {
                    ModelState.AddModelError(
                        $"Items[{i}].ProductId",
                        $"Item {i + 1}: the selected product does not exist or is inactive.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            var quote = new Quote
            {
                QuoteDate = model.QuoteDate,
                CustomerId = model.CustomerId!.Value, // checked by validation above
                BillingAddress = model.BillingAddress.Trim(),
                PaymentTerms = model.PaymentTerms.Trim(),
                SalesRepresentativeId = model.SalesRepresentativeId,
                QuoteItems = model.Items
                    .Select(i => new QuoteItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountPercent = i.DiscountPercent,
                        VatCategory = i.VatCategory
                    })
                    .ToList()
            };

            await _quoteService.CreateQuoteAsync(quote);

            TempData["SuccessMessage"] = $"Quote {quote.QuoteNumber} created successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync(QuoteViewModel model)
        {
            model.AvailableCustomers = await _context.Customers
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Include(c => c.SalesRepresentative)
                    .ThenInclude(sr => sr!.Employee)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .ToListAsync();

            model.AvailableProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    SellingPrice = p.SellingPrice
                })
                .ToListAsync();
        }

        // GET: Quotes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _quoteService.DeleteQuoteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}