

/***************************************************************************************
*    Title: Implement CRUD - ASP.NET MVC with Entity Framework Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud
***************************************************************************************/
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts supplier management to administrators and employees.
    [Authorize(Roles = "Administrator,Employee")]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;

        // Injects the supplier and product services through dependency injection.
        public SupplierController(
            ISupplierService supplierService,
            IProductService productService)
        {
            _supplierService = supplierService;
            _productService = productService;
        }

        // GET: Supplier/SupplierList
        [HttpGet]
        public async Task<IActionResult> SupplierList(string? search)
        {
            // Loads the supplier records used for the list and search.
            var suppliers = (await _supplierService.GetAllSuppliersAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                // Searches across the main supplier contact and business fields.
                suppliers = suppliers
                    .Where(s =>
                        s.CompanyName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        s.ContactPerson.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        s.ContactNumber.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        s.Email.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        s.PhysicalAddress.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        s.CreditTerms.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        s.VATNumber.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Search = search;

            // Loads all suppliers again to calculate the summary statistics.
            var allSuppliers =
                (await _supplierService.GetAllSuppliersAsync()).ToList();

            ViewBag.TotalSuppliers = allSuppliers.Count;
            ViewBag.ActiveSuppliers = allSuppliers.Count(s => s.IsActive);
            ViewBag.InactiveSuppliers = allSuppliers.Count(s => !s.IsActive);
            ViewBag.TotalCreditLimit = allSuppliers.Sum(s => s.CreditLimit);

            ViewBag.Search = search;

            return View("~/Views/Supplier/SupplierList.cshtml", suppliers);
        }

        // GET: Supplier/AddSupplier
        [HttpGet]
        public IActionResult AddSupplier()
        {
            // Displays an empty ViewModel for the new supplier form.
            return View(new SupplierViewModel());
        }

        // POST: Supplier/AddSupplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(SupplierViewModel model)
        {
            // Prevents invalid supplier data from being submitted.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _supplierService.CreateSupplierAsync(model);

            // Pop-up message shown on the next page (see _Layout.cshtml).
            TempData["ChangeMessage"] = $"Supplier \"{model.CompanyName}\" was added.";

            return RedirectToAction(nameof(SupplierList));
        }

        // GET: Supplier/EditSupplier/5
        [HttpGet]
        public async Task<IActionResult> EditSupplier(int id)
        {
            // Retrieves the supplier that will be edited.
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Supplier/EditSupplier/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, SupplierViewModel model)
        {
            // Ensures the route ID matches the supplier being edited.
            if (id != model.SupplierId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _supplierService.UpdateSupplierAsync(model);
            return RedirectToAction(nameof(SupplierList));
        }

        [HttpGet]
        public async Task<IActionResult> Products(int id)
        {
            // Prevents an invalid supplier ID from being used.
            if (id <= 0)
            {
                return NotFound();
            }

            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            // Gets products belonging to the selected supplier.
            var products = (await _productService.GetAllProductsAsync())
                .Where(p => p.SupplierId == id)
                .OrderBy(p => p.ProductName)
                .ToList();

            ViewBag.SupplierName = supplier.CompanyName;
            ViewBag.SupplierId = supplier.SupplierId;

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateSupplier(int id)
        {
            // Deactivates the supplier while retaining the supplier record.
            await _supplierService.DeactivateSupplierAsync(id);

            // Pop-up message shown on the next page (see _Layout.cshtml).
            TempData["ChangeMessage"] = "Supplier was deactivated.";

            return RedirectToAction(nameof(SupplierList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateSupplier(int id)
        {
            // Reactivates an existing supplier.
            await _supplierService.ActivateSupplierAsync(id);

            return RedirectToAction(nameof(SupplierList));
        }
    }
}