// Title: Controller for managing suppliers, supplier products and supplier status.
// Authors: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

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
        // Displays active suppliers and provides searching and summary statistics.
        [HttpGet]
        public async Task<IActionResult> SupplierList(string? search)
        {
            // Retrieves all suppliers so that active and inactive
            // supplier statistics can still be calculated.
            var allSuppliers =
                (await _supplierService.GetAllSuppliersAsync()).ToList();

            // The main supplier page displays active suppliers only.
            var suppliers = allSuppliers
                .Where(s => s.IsActive)
                .ToList();

            // Applies the search to active suppliers.
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

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

                        (s.Email != null &&
                         s.Email.Contains(
                             search,
                             StringComparison.OrdinalIgnoreCase)) ||

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

            // Makes the current search value available to the view.
            ViewBag.Search = search;

            // Summary statistics include all supplier records.
            ViewBag.TotalSuppliers =
                allSuppliers.Count;

            ViewBag.ActiveSuppliers =
                allSuppliers.Count(s => s.IsActive);

            ViewBag.InactiveSuppliers =
                allSuppliers.Count(s => !s.IsActive);

            // Credit limit only represents suppliers currently in use.
            ViewBag.TotalCreditLimit =
                allSuppliers
                    .Where(s => s.IsActive)
                    .Sum(s => s.CreditLimit);

            return View(
                "~/Views/Supplier/SupplierList.cshtml",
                suppliers);
        }


        // GET: Supplier/DeactivatedSuppliers
        // Displays suppliers that have been deactivated.
        [HttpGet]
        public async Task<IActionResult> DeactivatedSuppliers()
        {
            var suppliers =
                (await _supplierService.GetAllSuppliersAsync())
                .Where(s => !s.IsActive)
                .OrderBy(s => s.CompanyName)
                .ToList();

            return View(
                "~/Views/Supplier/DeactivatedSuppliers.cshtml",
                suppliers);
        }


        // GET: Supplier/AddSupplier
        // Displays the form used to add a new supplier.
        [HttpGet]
        public IActionResult AddSupplier()
        {
            return View(new SupplierViewModel());
        }


        // POST: Supplier/AddSupplier
        // Validates and creates a new supplier.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(
            SupplierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _supplierService.CreateSupplierAsync(model);

            TempData["SuccessMessage"] =
                "Supplier added successfully.";

            return RedirectToAction(nameof(SupplierList));
        }


        // GET: Supplier/EditSupplier/5
        // Retrieves and displays the selected supplier for editing.
        [HttpGet]
        public async Task<IActionResult> EditSupplier(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var supplier =
                await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }


        // POST: Supplier/EditSupplier/5
        // Validates and saves changes made to an existing supplier.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(
            int id,
            SupplierViewModel model)
        {
            if (id != model.SupplierId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _supplierService.UpdateSupplierAsync(model);

            TempData["SuccessMessage"] =
                "Supplier updated successfully.";

            return RedirectToAction(nameof(SupplierList));
        }


        // GET: Supplier/Products/5
        // Displays products associated with the selected supplier.
        [HttpGet]
        public async Task<IActionResult> Products(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var supplier =
                await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            var products =
                (await _productService.GetAllProductsAsync())
                .Where(p => p.SupplierId == id)
                .OrderBy(p => p.ProductName)
                .ToList();

            ViewBag.SupplierName =
                supplier.CompanyName;

            ViewBag.SupplierId =
                supplier.SupplierId;

            return View(products);
        }


        // POST: Supplier/DeactivateSupplier/5
        // Deactivates the supplier while retaining its historical record.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateSupplier(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var success =
                await _supplierService
                    .DeactivateSupplierAsync(id);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Supplier deactivated successfully.";

            return RedirectToAction(
                nameof(SupplierList));
        }


        // POST: Supplier/ActivateSupplier/5
        // Reactivates a previously deactivated supplier.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateSupplier(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var success =
                await _supplierService
                    .ActivateSupplierAsync(id);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Supplier activated successfully.";

            return RedirectToAction(
                nameof(DeactivatedSuppliers));
        }
    }
}