// Purpose: Supplier pages: list, search, add, edit, activate/deactivate and supplier products.
// Authors: iqran0906, Maseeha17, Naseeha27 (from git history)

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee")]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;

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
            var suppliers = (await _supplierService.GetAllSuppliersAsync()).ToList();

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
            return View(new SupplierViewModel());
        }

        // POST: Supplier/AddSupplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(SupplierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _supplierService.CreateSupplierAsync(model);
            return RedirectToAction(nameof(SupplierList));
        }

        // GET: Supplier/EditSupplier/5
        [HttpGet]
        public async Task<IActionResult> EditSupplier(int id)
        {
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
            if (id <= 0)
            {
                return NotFound();
            }

            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

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
            await _supplierService.DeactivateSupplierAsync(id);

            return RedirectToAction(nameof(SupplierList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateSupplier(int id)
        {
            await _supplierService.ActivateSupplierAsync(id);

            return RedirectToAction(nameof(SupplierList));
        }
    }
}