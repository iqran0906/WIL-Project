using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class ForecastingController : Controller
    {
        private readonly IForecastingService _forecastingService;
        private readonly ISupplierService _supplierService;

        public ForecastingController(IForecastingService forecastingService, ISupplierService supplierService)
        {
            _forecastingService = forecastingService;
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index(string? category, string? statusFilter)
        {
            var viewModel = await _forecastingService.GetFilteredForecastsAsync(category, statusFilter);
            return View(viewModel);
        }

        public async Task<IActionResult> ExportCsv(string? category, string? statusFilter)
        {
            var bytes = await _forecastingService.ExportForecastCsvAsync(category, statusFilter);
            return File(bytes, "text/csv", $"Inventory_Forecast_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        public async Task<IActionResult> Reorder(int productId)
        {
            var model = await _forecastingService.GetReorderModelAsync(productId);
            if (model == null) return NotFound();

            var suppliers = await _supplierService.GetAllSuppliersAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "SupplierName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder(TriggerReorderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "SupplierName");
                return View(model);
            }

            var success = await _forecastingService.ProcessReorderAsync(model);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = $"Reorder submitted for {model.ProductName}. Stock increased by {model.ReorderQuantity}.";
            return RedirectToAction(nameof(Index));
        }
    }
}