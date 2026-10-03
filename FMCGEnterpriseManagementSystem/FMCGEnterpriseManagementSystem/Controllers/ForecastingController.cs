/***************************************************************************************
*    Title: Forecasting Controller
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Controllers/ForecastingController.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only Administrators and Employees can access forecasting features.
    [Authorize(Roles = "Administrator,Employee")]

    public class ForecastingController : Controller
    {
        private readonly IForecastingService _forecastingService;
        private readonly ISupplierService _supplierService;

        // Dependency injection provides the forecasting and supplier services.
        public ForecastingController(
            IForecastingService forecastingService,
            ISupplierService supplierService)
        {
            _forecastingService = forecastingService;
            _supplierService = supplierService;
        }

        // Retrieves forecasts using the selected category and status filters.
        public async Task<IActionResult> Index(string? category, string? statusFilter)
        {
            var viewModel = await _forecastingService.GetFilteredForecastsAsync(
                category,
                statusFilter);

            return View(viewModel);
        }

        // Exports the filtered forecasting data as a CSV file.
        public async Task<IActionResult> ExportCsv(string? category, string? statusFilter)
        {
            var bytes = await _forecastingService.ExportForecastCsvAsync(
                category,
                statusFilter);

            return File(
                bytes,
                "text/csv",
                $"Inventory_Forecast_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        // Displays the reorder form for the selected product.
        public async Task<IActionResult> Reorder(int productId)
        {
            var model = await _forecastingService.GetReorderModelAsync(productId);

            // Return NotFound if the product does not have a reorder model.
            if (model == null)
                return NotFound();

            // Retrieve suppliers so the user can select an active supplier.
            var suppliers = await _supplierService.GetAllSuppliersAsync();

            ViewBag.Suppliers = new SelectList(
                suppliers.Where(s => s.IsActive),
                "SupplierId",
                "CompanyName"
            );

            return View(model);
        }

        // Processes the submitted reorder request.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder(TriggerReorderViewModel model)
        {
            // Return the form if the submitted data is invalid.
            if (!ModelState.IsValid)
            {
                // Reload the supplier list when returning to the form.
                var suppliers = await _supplierService.GetAllSuppliersAsync();

                ViewBag.Suppliers = new SelectList(
                    suppliers.Where(s => s.IsActive),
                    "SupplierId",
                    "CompanyName"
                );

                return View(model);
            }

            var success = await _forecastingService.ProcessReorderAsync(model);

            // Return NotFound if the reorder could not be processed.
            if (!success)
                return NotFound();

            TempData["SuccessMessage"] =
                $"Reorder submitted for {model.ProductName}. Stock increased by {model.ReorderQuantity}.";

            return RedirectToAction(nameof(Index));
        }
    }
}