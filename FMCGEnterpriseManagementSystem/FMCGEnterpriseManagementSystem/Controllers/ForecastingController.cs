using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

// Title: Controller for managing inventory forecasting and reorder operations.
// Authors: Maseeha17
// Date: 27-04-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only Administrators and Employees can access forecasting features.
    [Authorize(Roles = "Administrator,Employee")]
    public class ForecastingController : Controller
    {
        // Service responsible for forecasting and reorder business operations.
        private readonly IForecastingService _forecastingService;

        // Service responsible for retrieving supplier information.
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
            // Requests the filtered forecasting data from the forecasting service.
            var viewModel = await _forecastingService.GetFilteredForecastsAsync(
                category,
                statusFilter);

            // Sends the forecasting data to the associated View.
            return View(viewModel);
        }

        // Exports the filtered forecasting data as a CSV file.
        public async Task<IActionResult> ExportCsv(string? category, string? statusFilter)
        {
            // Generates the CSV data using the selected filters.
            var bytes = await _forecastingService.ExportForecastCsvAsync(
                category,
                statusFilter);

            // Returns the generated data as a downloadable CSV file.
            return File(
                bytes,
                "text/csv",
                $"Inventory_Forecast_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        // Displays the reorder form for the selected product.
        public async Task<IActionResult> Reorder(int productId)
        {
            // Retrieves the reorder information for the selected product.
            var model = await _forecastingService.GetReorderModelAsync(productId);

            // Returns NotFound if the product does not have a reorder model.
            if (model == null)
                return NotFound();

            // Retrieves suppliers so the user can select an active supplier.
            var suppliers = await _supplierService.GetAllSuppliersAsync();

            // Creates a dropdown containing only active suppliers.
            ViewBag.Suppliers = new SelectList(
                suppliers.Where(s => s.IsActive),
                "SupplierId",
                "CompanyName"
            );

            // Sends the reorder model to the View.
            return View(model);
        }

        // Processes the submitted reorder request.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder(TriggerReorderViewModel model)
        {
            // Checks whether the submitted reorder information is valid.
            if (!ModelState.IsValid)
            {
                // Reloads the supplier list when returning to the form.
                var suppliers = await _supplierService.GetAllSuppliersAsync();

                // Creates the supplier dropdown using only active suppliers.
                ViewBag.Suppliers = new SelectList(
                    suppliers.Where(s => s.IsActive),
                    "SupplierId",
                    "CompanyName"
                );

                // Returns the submitted model to the form so validation errors can be displayed.
                return View(model);
            }

            // Processes the reorder request through the forecasting service.
            var success = await _forecastingService.ProcessReorderAsync(model);

            // Returns NotFound if the reorder could not be processed.
            if (!success)
                return NotFound();

            // Stores a success message to be displayed after the redirect.
            TempData["SuccessMessage"] =
                $"Reorder submitted for {model.ProductName}. Stock increased by {model.ReorderQuantity}.";

            // Returns the user to the forecasting page after the reorder is completed.
            return RedirectToAction(nameof(Index));
        }
    }
}