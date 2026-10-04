using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

// Title: Controller for managing inventory items and stock adjustments.
// Authors: Microsoft
// Date: 27-04-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only Administrators and Employees can access inventory features.
    [Authorize(Roles = "Administrator,Employee")]
    public class InventoryController : Controller
    {
        // Service responsible for inventory-related business operations.
        private readonly IInventoryService _inventoryService;

        // Repository responsible for retrieving product information.
        private readonly IProductRepository _productRepository;

        // Dependency injection provides the inventory service and product repository.
        public InventoryController(
            IInventoryService inventoryService,
            IProductRepository productRepository)
        {
            _inventoryService = inventoryService;
            _productRepository = productRepository;
        }

        // Retrieves and displays all inventory items.
        public async Task<IActionResult> Index()
        {
            // Retrieves inventory records through the inventory service.
            var inventory = await _inventoryService.GetAllInventoryAsync();

            // Sends the inventory data to the View.
            return View(inventory);
        }

        // Displays the form for adding a new inventory item.
        [HttpGet]
        public async Task<IActionResult> AddItem()
        {
            // Load available products for the inventory form.
            var products = await _productRepository.GetAllAsync();

            // Makes the product list available to the View.
            ViewBag.Products = products;

            // Displays an empty inventory model for the form.
            return View(new InventoryViewModel());
        }

        // Processes the submitted inventory item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(InventoryViewModel model)
        {
            // Checks whether the submitted inventory data is valid.
            if (!ModelState.IsValid)
            {
                // Reloads the product list when validation fails.
                ViewBag.Products = await _productRepository.GetAllAsync();

                // Returns the submitted model to the form.
                return View(model);
            }

            try
            {
                // Creates the inventory item through the inventory service.
                await _inventoryService.CreateInventoryItemAsync(model);

                // Returns the user to the inventory list after successful creation.
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Displays the database or service error on the form.
                ModelState.AddModelError(
                    string.Empty,
                    $"Save failed: {ex.InnerException?.Message ?? ex.Message}"
                );

                // Reloads the product list before returning to the form.
                ViewBag.Products = await _productRepository.GetAllAsync();

                // Returns the submitted model with the error message.
                return View(model);
            }
        }

        // Retrieves an inventory item and displays it for editing.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieves the selected inventory item by its ID.
            var item = await _inventoryService.GetByIdAsync(id);

            // Return NotFound if the inventory item does not exist.
            if (item == null) return NotFound();

            // Sends the inventory item to the edit View.
            return View(item);
        }

        // Processes changes made to an inventory item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryViewModel model)
        {
            // Ensure the URL ID matches the submitted inventory ID.
            if (id != model.InventoryId) return NotFound();

            // Return the form if validation fails.
            if (!ModelState.IsValid) return View(model);

            // Updates the inventory item through the inventory service.
            await _inventoryService.UpdateInventoryItemAsync(model);

            // Returns the user to the inventory list.
            return RedirectToAction(nameof(Index));
        }

        // Displays the stock adjustment form.
        [HttpGet]
        public async Task<IActionResult> AdjustStock(int id)
        {
            // Retrieves the selected inventory item.
            var item = await _inventoryService.GetByIdAsync(id);

            // Return NotFound if the inventory item does not exist.
            if (item == null) return NotFound();

            // Populate the adjustment model with the current inventory information.
            var model = new AdjustStockViewModel
            {
                InventoryId = item.InventoryId,
                ProductName = item.ProductName,
                CurrentQuantity = item.QuantityOnHand
            };

            // Sends the populated adjustment model to the View.
            return View(model);
        }

        // Processes a stock quantity adjustment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(AdjustStockViewModel model)
        {
            // Return the form if the submitted data is invalid.
            if (!ModelState.IsValid) return View(model);

            // Updates the stock quantity through the inventory service.
            await _inventoryService.AdjustStockAsync(model);

            // Returns the user to the inventory list.
            return RedirectToAction(nameof(Index));
        }

        // Displays the confirmation page before deleting an inventory item.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieves the selected inventory item.
            var item = await _inventoryService.GetByIdAsync(id);

            // Return NotFound if the inventory item does not exist.
            if (item == null) return NotFound();

            // Sends the inventory item to the deletion confirmation View.
            return View(item);
        }

        // Deletes the selected inventory item.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Deletes the inventory item through the inventory service.
            await _inventoryService.DeleteInventoryItemAsync(id);

            // Returns the user to the inventory list after deletion.
            return RedirectToAction(nameof(Index));
        }
    }
}