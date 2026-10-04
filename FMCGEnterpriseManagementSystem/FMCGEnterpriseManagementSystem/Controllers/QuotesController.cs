

/***************************************************************************************
*    Title: Implement CRUD - ASP.NET MVC with Entity Framework Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core 10.0
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud
***************************************************************************************/

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
    // Restricts quote functionality to authorized business users.
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class QuotesController : Controller
    {
        // Quote service handles quote-related business operations.
        private readonly IQuoteService _quoteService;

        // Database context is used to load products, customers and sales representatives.
        private readonly ApplicationDbContext _context;

        // Dependencies are supplied through dependency injection.
        public QuotesController(
            IQuoteService quoteService,
            ApplicationDbContext context)
        {
            _quoteService = quoteService;
            _context = context;
        }

        // Loads the dropdown data required by the create and edit quote forms.
        private async Task PopulateQuoteDropdownsAsync(
            int? selectedSalesRepId = null,
            string? selectedPaymentTerms = null)
        {
            // Provides the available payment-term options.
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

            // Loads active products for quote line items.
            ViewBag.ProductList = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            // Loads active customers for the quote customer selection.
            ViewBag.CustomerList = await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .ToListAsync();

            // Loads active sales representatives for the sales representative dropdown.
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
        // Displays all quotes.
        public async Task<IActionResult> Index()
        {
            var quotes = await _quoteService.GetAllQuotesAsync();

            return View(quotes);
        }

        // GET: Quotes/Create
        // Displays the form for creating a new quote.
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
        // Validates and creates a new quote.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quote quote)
        {
            // Navigation properties are not submitted as part of the quote form,
            // so they are removed from MVC validation.
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

            // A quote must contain at least one product.
            if (quote.QuoteItems == null || !quote.QuoteItems.Any())
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please add at least one product to the quote.");
            }

            // Every quote item must have a valid product selected.
            if (quote.QuoteItems != null &&
                quote.QuoteItems.Any(item => item.ProductId <= 0))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please select a product for every quote item.");
            }

            // Reload dropdowns when validation fails so the form can be displayed again.
            if (!ModelState.IsValid)
            {
                await PopulateQuoteDropdownsAsync(
                    quote.SalesRepresentativeId,
                    quote.PaymentTerms);

                return View(quote);
            }

            await _quoteService.CreateQuoteAsync(quote);

            // Pop-up message shown on the next page (see _Layout.cshtml).
            // The quote service fills in the quote number when it saves.
            TempData["ChangeMessage"] = $"Quote {quote.QuoteNumber} was added.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Quotes/Edit/5
        // Loads an existing quote for editing.
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
        // Validates and saves changes to an existing quote.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quote quote)
        {
            // Ensures the route ID matches the quote being edited.
            if (id != quote.QuoteId)
            {
                return BadRequest();
            }

            // Navigation and generated properties are not submitted by the form.
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

            // A quote must contain at least one product.
            if (quote.QuoteItems == null || !quote.QuoteItems.Any())
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please add at least one product to the quote.");
            }

            // Every quote item must have a valid product selected.
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
        // Converts the selected quote into an invoice through the quote service.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToInvoice(int id)
        {
            await _quoteService.ConvertToInvoiceAsync(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Quotes/DownloadPdf/5
        // Generates and downloads a PDF copy of the selected quote.
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
        // Displays the details of a selected quote.
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
        // Deletes the selected quote through the quote service.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _quoteService.DeleteQuoteAsync(id);

            // Pop-up message shown on the next page (see _Layout.cshtml).
            if (deleted)
            {
                TempData["ChangeMessage"] = "Quote was deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}