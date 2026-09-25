using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.QuantityInStock <= p.ReorderLevel)
                .OrderBy(p => p.QuantityInStock)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await Task.CompletedTask;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}