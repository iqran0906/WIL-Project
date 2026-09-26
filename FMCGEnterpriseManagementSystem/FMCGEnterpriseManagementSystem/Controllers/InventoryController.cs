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
    }
}