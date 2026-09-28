// Purpose: Controller for the invoices pages and form submissions.
// Authors: iqran0906, Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using FMCGEnterpriseManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ApplicationDbContext _context;

        public InvoicesController(IInvoiceService invoiceService, ICustomerRepository customerRepository, ApplicationDbContext context)
        {
            _invoiceService = invoiceService;
            _customerRepository = customerRepository;
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index(int? customerId, DateTime? startDate, DateTime? endDate, string? keyword)
        {
            IEnumerable<InvoiceViewModel> invoices;

            if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
            {
                ViewBag.DateError = "The end date cannot be before the start date.";
                invoices = Enumerable.Empty<InvoiceViewModel>();
            }
            else
            {
                invoices = await _invoiceService.SearchAsync(customerId, startDate, endDate, keyword);
            }

            // Filter options and the values currently applied
            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .Select(c => new
                {
                    c.CustomerId,
                    Display = c.Name + " " + c.Surname + " (No. " + c.CustomerId + ")"
                })
                .ToListAsync();

            ViewBag.CustomerFilter = new SelectList(customers, "CustomerId", "Display", customerId);
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Keyword = keyword;

            return View(invoices);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _invoiceService.GetByIdAsync(id.Value);
            if (invoice == null) return NotFound();

            return View(invoice);
        }
        // GET: Invoices/Create
        public async Task<IActionResult> Create()
        {
            var model = new InvoiceViewModel
            {
                InvoiceDate = DateTime.Today
            };

            await LoadLookupsAsync(model);

            return View(model);
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            // Rules that need the database
            if (model.CustomerId > 0 &&
                !await _context.Customers.AnyAsync(c => c.CustomerId == model.CustomerId && c.IsActive))
            {
                ModelState.AddModelError(nameof(model.CustomerId), "The selected customer does not exist or is inactive.");
            }

            await ValidateProductsAsync(model.Items);

            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            try
            {
                await _invoiceService.CreateAsync(model);

                TempData["SuccessMessage"] = "Invoice created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Business rule failures from the service, e.g. insufficient stock
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadLookupsAsync(model);
                return View(model);
            }
        }

        private async Task LoadLookupsAsync(InvoiceViewModel model)
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

        private async Task ValidateProductsAsync(IEnumerable<InvoiceItemViewModel> items)
        {
            var productIds = items.Select(i => i.ProductId).Where(id => id > 0).Distinct().ToList();

            var activeIds = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && p.IsActive)
                .Select(p => p.ProductId)
                .ToListAsync();

            var index = 0;
            foreach (var item in items)
            {
                if (item.ProductId > 0 && !activeIds.Contains(item.ProductId))
                {
                    ModelState.AddModelError(
                        $"Items[{index}].ProductId",
                        $"Item {index + 1}: the selected product does not exist or is inactive.");
                }
                index++;
            }
        }

        // POST: Invoices/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                await _invoiceService.UpdateStatusAsync(id, status);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Invoices/Download/5
        public async Task<IActionResult> Download(int id)
        {
            // TODO: Replace with real PDF generation once the Exports module is built
            TempData["InfoMessage"] = "Invoice download (PDF export) is coming soon.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _invoiceService.GetByIdAsync(id.Value);
            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _invoiceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}