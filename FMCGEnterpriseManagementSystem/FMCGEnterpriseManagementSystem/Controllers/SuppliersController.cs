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

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: Supplier/SupplierList
        [HttpGet]
        public async Task<IActionResult> SupplierList()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
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

        // GET: Supplier/DeleteSupplier/5
        [HttpGet]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Supplier/DeleteSupplierConfirmed
        [HttpPost, ActionName("DeleteSupplierConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplierConfirmed(int supplierId)
        {
            await _supplierService.DeleteSupplierAsync(supplierId);
            return RedirectToAction(nameof(SupplierList));
        }
    }
}