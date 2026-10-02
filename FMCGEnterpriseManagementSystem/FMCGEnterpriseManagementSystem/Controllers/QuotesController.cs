using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Enums;
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
        public async Task<IActionResult> Index()
        {
            var quotes = await _quoteService.GetAllQuotesAsync();

            return View(quotes);
        }

        // GET: Quotes/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateQuoteDropdownsAsync();

            var quote = new Quote
            {
                QuoteDate = DateTime.Today
            };

            return View(quote);
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quote quote)
        {
            ModelState.Remove(nameof(Quote.QuoteNumber));
            ModelState.Remove(nameof(Quote.Customer));
            ModelState.Remove(nameof(Quote.SalesRepresentative));

            if (quote.QuoteItems != null)
            {
                for (int i = 0; i < quote.QuoteItems.Count; i++)
                {
                    ModelState.Remove($"QuoteItems[{i}].Quote");
                    ModelState.Remove($"QuoteItems[{i}].Product");
                }
            }

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
                await PopulateQuoteDropdownsAsync(
                    quote.SalesRepresentativeId,
                    quote.PaymentTerms);

                return View(quote);
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quote quote)
        {
            if (id != quote.QuoteId)
            {
                return BadRequest();
            }

            ModelState.Remove(nameof(Quote.QuoteNumber));
            ModelState.Remove(nameof(Quote.Customer));
            ModelState.Remove(nameof(Quote.SalesRepresentative));

            if (quote.QuoteItems != null)
            {
                for (int i = 0; i < quote.QuoteItems.Count; i++)
                {
                    ModelState.Remove($"QuoteItems[{i}].Quote");
                    ModelState.Remove($"QuoteItems[{i}].Product");
                }
            }

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
                await PopulateQuoteDropdownsAsync(
                    quote.SalesRepresentativeId,
                    quote.PaymentTerms);

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