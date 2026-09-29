using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class QuotesController : Controller
    {
        private readonly IQuoteService _quoteService;
        private readonly ApplicationDbContext _context;

        public QuotesController(
            IQuoteService quoteService,
            ApplicationDbContext context)
        {
            _quoteService = quoteService;
            _context = context;
        }

        private async Task PopulateQuoteDropdownsAsync(
            int? selectedSalesRepId = null,
            string? selectedPaymentTerms = null)
        {
            ViewBag.PaymentTermsList = new SelectList(
                new[]
                {
                    "COD",
                    "7 Days",
                    "14 Days",
                    "21 Days",
                    "28 Days",
                    "30 Days"
                },
                selectedPaymentTerms);

            ViewBag.ProductList = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            ViewBag.CustomerList = await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .ToListAsync();

            var salesReps = await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .Where(sr => sr.IsActive)
                .OrderBy(sr => sr.Employee.FirstName)
                .ThenBy(sr => sr.Employee.LastName)
                .Select(sr => new
                {
                    sr.SalesRepresentativeId,

                    DisplayName =
                        sr.Employee.FirstName + " " +
                        sr.Employee.LastName + " (" +
                        sr.SalesRepCode + ")"
                })
                .ToListAsync();

            ViewBag.SalesRepList = new SelectList(
                salesReps,
                "SalesRepresentativeId",
                "DisplayName",
                selectedSalesRepId);
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

        // GET: Quotes/Create
        [HttpGet]
        public async Task<IActionResult> Create()
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

            await PopulateQuoteDropdownsAsync();

            return View(quote);
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuoteViewModel model)
        {
            ModelState.Remove(nameof(Quote.QuoteNumber));
            ModelState.Remove("Customer");
            ModelState.Remove("SalesRepresentative");

            if (!ModelState.IsValid)
            {
                await PopulateQuoteDropdownsAsync(
                    quote.SalesRepresentativeId,
                    quote.PaymentTerms);

                return View(quote);
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

            return RedirectToAction(nameof(Index));
        }

        // GET: Quotes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            await PopulateQuoteDropdownsAsync(
                quote.SalesRepresentativeId,
                quote.PaymentTerms);

            return View(quote);
        }

        // POST: Quotes/Edit/5
        // POST: Quotes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quote quote)
        {
            if (id != quote.QuoteId)
            {
                return BadRequest();
            }

            // These values are generated/loaded by the application,
            // not entered directly by the user.
            ModelState.Remove(nameof(Quote.QuoteNumber));
            ModelState.Remove(nameof(Quote.Customer));
            ModelState.Remove(nameof(Quote.SalesRepresentative));

            // QuoteItem navigation properties are not posted by the form.
            // Only ProductId is posted.
            for (int i = 0; i < quote.QuoteItems.Count; i++)
            {
                ModelState.Remove($"QuoteItems[{i}].Quote");
                ModelState.Remove($"QuoteItems[{i}].Product");
            }

            // A quote must contain at least one valid item.
            if (quote.QuoteItems == null || !quote.QuoteItems.Any())
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please add at least one product to the quote.");
            }

            if (quote.QuoteItems != null &&
                quote.QuoteItems.Any(item => item.ProductId <= 0))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please select a product for every quote item.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PaymentTermsList = new SelectList(
                    new[]
                    {
                "COD",
                "7 Days",
                "14 Days",
                "21 Days",
                "28 Days",
                "30 Days"
                    },
                    quote.PaymentTerms);

                ViewBag.ProductList = await _context.Products
                    .Where(p => p.IsActive)
                    .ToListAsync();

                ViewBag.CustomerList = await _context.Customers
                    .Where(c => c.IsActive)
                    .ToListAsync();

                ViewBag.SalesRepList = new SelectList(
    await _context.SalesRepresentatives
        .Include(sr => sr.Employee)
        .Where(sr => sr.IsActive)
        .Select(sr => new
        {
            sr.SalesRepresentativeId,
            DisplayName = sr.Employee.FirstName + " " +
                          sr.Employee.LastName + " (" +
                          sr.SalesRepCode + ")"
        })
        .ToListAsync(),
    "SalesRepresentativeId",
    "DisplayName",
    quote.SalesRepresentativeId
);

                return View(quote);
            }

            quote.QuoteId = id;

            await _quoteService.UpdateQuoteAsync(quote);

            return RedirectToAction(nameof(Index));
        }

        // POST: Quotes/ConvertToInvoice/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToInvoice(int id)
        {
            await _quoteService.ConvertToInvoiceAsync(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Quotes/DownloadPdf/5
        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            var pdfBytes = QuotePdfGenerator.Generate(quote);

            return File(
                pdfBytes,
                "application/pdf",
                $"Quote-{quote.QuoteNumber}.pdf");
        }

        // GET: Quotes/Details/5
        [HttpGet]
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