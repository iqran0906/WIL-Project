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
        public async Task<IActionResult> Index()
        {
            var quotes = await _quoteService.GetAllQuotesAsync();
            return View(quotes);
        }

        // GET: Quotes/Create
        public IActionResult Create()
        {
            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddDays(30)
            };
            return View(quote);
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return View(quote);
            }

            await _quoteService.CreateQuoteAsync(quote);
            return RedirectToAction(nameof(Index));
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