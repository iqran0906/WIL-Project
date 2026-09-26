using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class QuotesController : Controller
    {
        private readonly IQuoteService _quoteService;
        private readonly IEmailApiClientService _emailApiClientService;

        public QuotesController(IQuoteService quoteService, IEmailApiClientService emailApiClientService)
        {
            _quoteService = quoteService;
            _emailApiClientService = emailApiClientService;
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
                QuoteDate = DateTime.Today
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

        // POST: Quotes/EmailQuote/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailQuote(int id, string recipientEmail)
        {
            var result = await _emailApiClientService.EmailQuoteAsync(id, recipientEmail);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
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