// Purpose: Controller for the products pages and form submissions.
// Authors: iqran0906, Maseeha17 (from git history)

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;

        public ProductsController(
            IProductService productService,
            ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        public async Task<IActionResult> Index(
     string? search,
     string? category,
     int? supplierId,
     string? status)
        {
            var products = (await _productService.GetAllProductsAsync()).ToList();


            ViewBag.TotalProducts = products.Count; // Summary cards use the complete product list
            ViewBag.ActiveProducts = products.Count(p => p.IsActive);
            ViewBag.InactiveProducts = products.Count(p => !p.IsActive);
            ViewBag.TotalSuppliers = products
                .Select(p => p.SupplierId)
                .Distinct()
                .Count();

           
            if (!string.IsNullOrWhiteSpace(search))  // Search
            {
                search = search.Trim();

                products = products
                    .Where(p =>
                        (p.ProductCode?.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ?? false) ||
                        p.ProductName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||
                        (p.Description?.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ?? false) ||
                        p.Category.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||
                        p.SupplierName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            
            if (!string.IsNullOrWhiteSpace(category)) // Category filter
            {
                products = products
                    .Where(p => p.Category == category)
                    .ToList();
            }

            
            if (supplierId.HasValue) // Supplier filter
            {
                products = products
                    .Where(p => p.SupplierId == supplierId.Value)
                    .ToList();
            }

           
            if (status == "active")  // Status filter
            {
                products = products
                    .Where(p => p.IsActive)
                    .ToList();
            }
            else if (status == "inactive")
            {
                products = products
                    .Where(p => !p.IsActive)
                    .ToList();
            }

           
            ViewBag.Categories = await _context.Products  // Filter options
                .AsNoTracking()
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.CompanyName)
                .ToListAsync();

            ViewBag.SupplierFilter = new SelectList(
                suppliers,
                "SupplierId",
                "CompanyName",
                supplierId);

            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedStatus = status;

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            await LoadSuppliersAsync();
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadSuppliersAsync();
                return View(model);
            }

            await _productService.CreateProductAsync(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await LoadSuppliersAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ProductViewModel model)
        {
            if (id != model.ProductId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadSuppliersAsync();
                return View(model);
            }

            await _productService.UpdateProductAsync(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (id <= 0)
                return NotFound();

            await _productService.ActivateProductAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSuppliersAsync()
        {
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.CompanyName)
                .Select(s => new
                {
                    s.SupplierId,
                    DisplayName = $"Supplier {s.SupplierId} - {s.CompanyName}"
                })
                .ToListAsync();

            ViewBag.Suppliers = new SelectList(
                suppliers,
                "SupplierId",
                "DisplayName");
        }
    }
}