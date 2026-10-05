// Title: Implement CRUD - ASP.NET MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts product management to administrators and employees.
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

        // Displays active products with search and filter options.
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? category,
            int? supplierId)
        {
            var allProducts =
                (await _productService.GetAllProductsAsync()).ToList();

            // Main product page only displays active products.
            var products = allProducts
                .Where(p => p.IsActive)
                .ToList();

            // Summary information.
            ViewBag.TotalProducts = allProducts.Count;

            ViewBag.ActiveProducts =
                allProducts.Count(p => p.IsActive);

            ViewBag.InactiveProducts =
                allProducts.Count(p => !p.IsActive);

            ViewBag.TotalSuppliers = allProducts
                .Where(p => p.IsActive)
                .Select(p => p.SupplierId)
                .Distinct()
                .Count();

            // Search active products.
            if (!string.IsNullOrWhiteSpace(search))
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

                        (p.SupplierName?.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            // Category filter.
            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products
                    .Where(p => p.Category == category)
                    .ToList();
            }

            // Supplier filter.
            if (supplierId.HasValue)
            {
                products = products
                    .Where(p => p.SupplierId == supplierId.Value)
                    .ToList();
            }

            // Load categories.
            ViewBag.Categories = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // Only active suppliers can be selected.
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

            return View(products);
        }

        // Displays products that are no longer active.
        [HttpGet]
        public async Task<IActionResult> DiscontinuedProducts()
        {
            var products =
                (await _productService.GetAllProductsAsync())
                .Where(p => !p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();

            return View(products);
        }

        // Displays the form for creating a new product.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadSuppliersAsync();

            return View(new ProductViewModel());
        }

        // Creates a new product.
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

            TempData["SuccessMessage"] =
                "Product added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Displays an existing product for editing.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var product =
                await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await LoadSuppliersAsync();

            return View(product);
        }

        // Updates an existing product.
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

            TempData["SuccessMessage"] =
                "Product updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Displays the confirmation page before discontinuing a product.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var product =
                await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Discontinues the product without physically deleting it.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var product =
                await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);

            TempData["SuccessMessage"] =
                "Product discontinued successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Reactivates a discontinued product.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var product =
                await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productService.ActivateProductAsync(id);

            TempData["SuccessMessage"] =
                "Product reactivated successfully.";

            return RedirectToAction(
                nameof(DiscontinuedProducts));
        }

        // Loads active suppliers for product forms.
        private async Task LoadSuppliersAsync()
        {
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.CompanyName)
                .Select(s => new
                {
                    s.SupplierId,
                    DisplayName =
                        $"Supplier {s.SupplierId} - {s.CompanyName}"
                })
                .ToListAsync();

            ViewBag.Suppliers = new SelectList(
                suppliers,
                "SupplierId",
                "DisplayName");
        }
    }
}