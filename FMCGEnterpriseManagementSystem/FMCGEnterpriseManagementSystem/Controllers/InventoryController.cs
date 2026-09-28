using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index()
        {
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return View(inventory);
        }

        [HttpGet]
        public IActionResult AddItem()
        {
            return View(new InventoryViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(InventoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _inventoryService.CreateInventoryItemAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // This catches the exact database/service error and displays it on your form
                ModelState.AddModelError(string.Empty, $"Save failed: {ex.InnerException?.Message ?? ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryViewModel model)
        {
            if (id != model.InventoryId) return NotFound();

            if (!ModelState.IsValid) return View(model);

            await _inventoryService.UpdateInventoryItemAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AdjustStock(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();

            var model = new AdjustStockViewModel
            {
                InventoryId = item.InventoryId,
                ProductName = item.ProductName,
                CurrentQuantity = item.QuantityOnHand
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(AdjustStockViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _inventoryService.AdjustStockAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _inventoryService.DeleteInventoryItemAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}