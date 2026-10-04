

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only Administrators and Employees can access inventory features.
    [Authorize(Roles = "Administrator,Employee")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
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
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return View(inventory);
        }

        // Displays the form for adding a new inventory item.
        [HttpGet]
        public async Task<IActionResult> AddItem()
        {
            // Load available products for the inventory form.
            var products = await _productRepository.GetAllAsync();

            ViewBag.Products = products;

            return View(new InventoryViewModel());
        }

        // Processes the submitted inventory item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(InventoryViewModel model)
        {
            // Return the form if the submitted data is invalid.
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _productRepository.GetAllAsync();
                return View(model);
            }

            try
            {
                // Create the inventory item through the inventory service.
                await _inventoryService.CreateInventoryItemAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Display the database or service error on the form.
                ModelState.AddModelError(
                    string.Empty,
                    $"Save failed: {ex.InnerException?.Message ?? ex.Message}"
                );

                // Reload the product list before returning to the form.
                ViewBag.Products = await _productRepository.GetAllAsync();

                return View(model);
            }
        }

        // Retrieves an inventory item and displays it for editing.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);

            // Return NotFound if the inventory item does not exist.
            if (item == null) return NotFound();

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

            await _inventoryService.UpdateInventoryItemAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // Displays the stock adjustment form.
        [HttpGet]
        public async Task<IActionResult> AdjustStock(int id)
        {
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

            return View(model);
        }

        // Processes a stock quantity adjustment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(AdjustStockViewModel model)
        {
            // Return the form if the submitted data is invalid.
            if (!ModelState.IsValid) return View(model);

            await _inventoryService.AdjustStockAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // Displays the confirmation page before deleting an inventory item.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);

            // Return NotFound if the inventory item does not exist.
            if (item == null) return NotFound();

            return View(item);
        }

        // Deletes the selected inventory item.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _inventoryService.DeleteInventoryItemAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}