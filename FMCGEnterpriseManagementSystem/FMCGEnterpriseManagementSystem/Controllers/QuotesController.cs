using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class QuotesController : Controller
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        // GET: Quotes
        public async Task<IActionResult> Index(string? keyword, string? customer, DateTime? startDate, DateTime? endDate)
        {
            var quotes = await _quoteService.GetAllQuotesAsync();
            var query = quotes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(q => (q.QuoteNumber != null && q.QuoteNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                         (q.Status.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(customer))
            {
                query = query.Where(q => q.Customer != null && q.Customer.Email != null && q.Customer.Email.Contains(customer, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue) query = query.Where(q => q.QuoteDate >= startDate.Value);
            if (endDate.HasValue) query = query.Where(q => q.QuoteDate <= endDate.Value);

            ViewBag.Keyword = keyword;
            ViewBag.Customer = customer;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(query.ToList());
        }

        // GET: Quotes/Create
        public IActionResult Create()
        {
            var quote = new Quote { QuoteDate = DateTime.Today };
            return View(quote);
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quote quote)
        {
            // Server-side model state validation
            if (!ModelState.IsValid)
            {
                return View(quote);
            }

            try
            {
                await _quoteService.CreateQuoteAsync(quote);
                TempData["SuccessMessage"] = "Quote created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Graceful error handling for business logic exceptions
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(quote);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving the quote.");
                return View(quote);
            }
        }

        // GET: Quotes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);
            if (quote == null) return NotFound();
            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _quoteService.DeleteQuoteAsync(id);
                TempData["SuccessMessage"] = "Quote deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting quote: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}