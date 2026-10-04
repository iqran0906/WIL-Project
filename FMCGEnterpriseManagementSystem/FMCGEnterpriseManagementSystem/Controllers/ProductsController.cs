

/***************************************************************************************
*    Title: Implement CRUD - ASP.NET MVC with Entity Framework Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core 10.0
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud
***************************************************************************************/

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
        // Service handles product-related business operations.
        private readonly IProductService _productService;

        // Database context is used for supplier and category lookup data.
        private readonly ApplicationDbContext _context;

        // Dependencies are supplied through dependency injection.
        public ProductsController(
            IProductService productService,
            ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        // Displays the product list with search and filter options.
        public async Task<IActionResult> Index(
     string? search,
     string? category,
     int? supplierId,
     string? status)
        {
            // Retrieve the complete product list before applying display filters.
            var products = (await _productService.GetAllProductsAsync()).ToList();


            // Summary cards use the complete product list.
            ViewBag.TotalProducts = products.Count;
            ViewBag.ActiveProducts = products.Count(p => p.IsActive);
            ViewBag.InactiveProducts = products.Count(p => !p.IsActive);
            ViewBag.TotalSuppliers = products
                .Select(p => p.SupplierId)
                .Distinct()
                .Count();


            // Apply keyword search across product and supplier information.
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
                        p.SupplierName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }


            // Apply the selected product category filter.
            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products
                    .Where(p => p.Category == category)
                    .ToList();
            }


            // Apply the selected supplier filter.
            if (supplierId.HasValue)
            {
                products = products
                    .Where(p => p.SupplierId == supplierId.Value)
                    .ToList();
            }


            // Apply the active or inactive product status filter.
            if (status == "active")
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


            // Load distinct categories used by the product filter.
            ViewBag.Categories = await _context.Products
                .AsNoTracking()
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // Load active suppliers for the supplier filter.
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

            // Preserve the selected filter values for the view.
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedStatus = status;

            return View(products);
        }

        // Displays the form for creating a new product.
        public async Task<IActionResult> Create()
        {
            await LoadSuppliersAsync();
            return View(new ProductViewModel());
        }

        // Processes a new product submission.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            // Prevent invalid product information from being submitted.
            if (!ModelState.IsValid)
            {
                await LoadSuppliersAsync();
                return View(model);
            }

            await _productService.CreateProductAsync(model);

            // Pop-up message shown on the next page (see _Layout.cshtml).
            TempData["ChangeMessage"] = $"Product \"{model.ProductName}\" was added.";

            return RedirectToAction(nameof(Index));
        }

        // Loads an existing product for editing.
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

        // Processes changes made to an existing product.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ProductViewModel model)
        {
            // Ensure the route ID matches the product being edited.
            if (id != model.ProductId)
            {
                return BadRequest();
            }

            // Return the form if validation fails.
            if (!ModelState.IsValid)
            {
                await LoadSuppliersAsync();
                return View(model);
            }

            await _productService.UpdateProductAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // Displays the confirmation page before deleting a product.
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

        // Processes the confirmed product deletion.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);

            // Pop-up message shown on the next page (see _Layout.cshtml).
            TempData["ChangeMessage"] = "Product was removed (marked inactive).";

            return RedirectToAction(nameof(Index));
        }

        // Re-activates a previously inactive product.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (id <= 0)
                return NotFound();

            await _productService.ActivateProductAsync(id);

            return RedirectToAction(nameof(Index));
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