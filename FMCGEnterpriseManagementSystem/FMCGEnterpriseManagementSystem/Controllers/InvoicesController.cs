using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using FMCGEnterpriseManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
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
        public async Task<IActionResult> Index(int? customerId, DateTime? startDate, DateTime? endDate, string keyword)
        {
            var invoices = await _invoiceService.SearchAsync(customerId, startDate, endDate, keyword);
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
            var customers = await _context.Customers
     .Include(c => c.SalesRepresentative)
         .ThenInclude(sr => sr.Employee)
     .ToListAsync();

            return View(new InvoiceViewModel
            {
                InvoiceDate = DateTime.Today,
                AvailableCustomers = customers
            });
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _invoiceService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
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