

/***************************************************************************************
*    Title: Implement CRUD - ASP.NET MVC with Entity Framework Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core 10.0
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud
***************************************************************************************/

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
    // Only Administrators, Employees and Sales Representatives can access invoices.
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerRepository _customerRepository;

        private readonly IInvoiceExportService _invoiceExportService;

        private readonly ApplicationDbContext _context;

        // Dependency injection provides the required invoice, customer,
        // export and database services.
        public InvoicesController(
     IInvoiceService invoiceService,
     IInvoiceExportService invoiceExportService,
     ICustomerRepository customerRepository,
     ApplicationDbContext context)
        {
            _invoiceService = invoiceService;
            _invoiceExportService = invoiceExportService;
            _customerRepository = customerRepository;
            _context = context;
        }

        // GET: Invoices
        // Displays invoices using optional customer, date and keyword filters.
        public async Task<IActionResult> Index(
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            string? keyword)
        {
            IEnumerable<InvoiceViewModel> invoices;

            // Prevent an invalid date range from being used for the search.
            if (startDate.HasValue &&
                endDate.HasValue &&
                startDate.Value.Date > endDate.Value.Date)
            {
                ViewBag.DateError =
                    "The end date cannot be before the start date.";

                invoices = Enumerable.Empty<InvoiceViewModel>();
            }
            else
            {
                invoices = await _invoiceService.SearchAsync(
                    customerId,
                    startDate,
                    endDate,
                    keyword);
            }

            // Load customer filter options and the values currently applied.
            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .Select(c => new
                {
                    c.CustomerId,
                    Display =
                        c.Name + " " +
                        c.Surname +
                        " (No. " +
                        c.CustomerId +
                        ")"
                })
                .ToListAsync();

            ViewBag.CustomerFilter =
                new SelectList(
                    customers,
                    "CustomerId",
                    "Display",
                    customerId);

            ViewBag.StartDate =
                startDate?.ToString("yyyy-MM-dd");

            ViewBag.EndDate =
                endDate?.ToString("yyyy-MM-dd");

            ViewBag.Keyword = keyword;

            return View(invoices);
        }

        // GET: Invoices/Details/5
        // Displays the details of a selected invoice.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var invoice =
                await _invoiceService.GetByIdAsync(id.Value);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // GET: Invoices/Create
        // Displays the invoice creation form.
        public async Task<IActionResult> Create()
        {
            var model = new InvoiceViewModel
            {
                InvoiceDate = DateTime.Today
            };

            // Load customers and products for the invoice form.
            await LoadLookupsAsync(model);

            return View(model);
        }

        // POST: Invoices/Create
        // Processes the submitted invoice.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            InvoiceViewModel model)
        {
            // Validate that the selected customer exists and is active.
            if (model.CustomerId > 0 &&
                !await _context.Customers.AnyAsync(
                    c => c.CustomerId == model.CustomerId &&
                         c.IsActive))
            {
                ModelState.AddModelError(
                    nameof(model.CustomerId),
                    "The selected customer does not exist or is inactive.");
            }

            // Validate the products included in the invoice.
            await ValidateProductsAsync(model.Items);

            // Return the form if validation fails.
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            try
            {
                await _invoiceService.CreateAsync(model);

                TempData["SuccessMessage"] =
                    "Invoice created successfully.";

                // Pop-up message shown on the next page (see _Layout.cshtml).
                TempData["ChangeMessage"] = "Invoice was added.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Handles business rule failures from the service,
                // such as insufficient stock.
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await LoadLookupsAsync(model);

                return View(model);
            }
        }

        // Loads the customer and product information required by the invoice form.
        private async Task LoadLookupsAsync(
            InvoiceViewModel model)
        {
            model.AvailableCustomers =
                await _context.Customers
                    .AsNoTracking()
                    .Where(c => c.IsActive)
                    .Include(c => c.SalesRepresentative)
                        .ThenInclude(sr => sr!.Employee)
                    .OrderBy(c => c.Name)
                    .ThenBy(c => c.Surname)
                    .ToListAsync();

            model.AvailableProducts =
                await _context.Products
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

        // Checks that the products submitted with the invoice are valid and active.
        private async Task ValidateProductsAsync(
            IEnumerable<InvoiceItemViewModel> items)
        {
            var productIds =
                items
                    .Select(i => i.ProductId)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

            var activeIds =
                await _context.Products
                    .Where(p =>
                        productIds.Contains(p.ProductId) &&
                        p.IsActive)
                    .Select(p => p.ProductId)
                    .ToListAsync();

            var index = 0;

            foreach (var item in items)
            {
                if (item.ProductId > 0 &&
                    !activeIds.Contains(item.ProductId))
                {
                    ModelState.AddModelError(
                        $"Items[{index}].ProductId",
                        $"Item {index + 1}: the selected product does not exist or is inactive.");
                }

                index++;
            }
        }

        // POST: Invoices/UpdateStatus/5
        // Only Administrators can approve and finalise invoices.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            string status)
        {
            try
            {
                await _invoiceService.UpdateStatusAsync(
                    id,
                    status);

                TempData["SuccessMessage"] =
                    "Invoice approved successfully. The invoice is now finalised.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }
        }

        // GET: Invoices/Download/5
        // Generates and downloads the invoice as a PDF document.
        public async Task<IActionResult> Download(int id)
        {
            var invoice =
                await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            var pdf =
                _invoiceExportService.GenerateInvoicePdf(invoice);

            return File(
                pdf,
                "application/pdf",
                $"Invoice-{invoice.InvoiceNumber}.pdf");
        }

        // GET: Invoices/Delete/5
        // Displays the invoice before deletion is confirmed.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var invoice =
                await _invoiceService.GetByIdAsync(id.Value);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        // Deletes the selected invoice.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            try
            {
                await _invoiceService.DeleteAsync(id);

                TempData["SuccessMessage"] =
                    "Invoice deleted successfully.";

                // Pop-up message shown on the next page (see _Layout.cshtml).
                TempData["ChangeMessage"] = "Invoice was deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}