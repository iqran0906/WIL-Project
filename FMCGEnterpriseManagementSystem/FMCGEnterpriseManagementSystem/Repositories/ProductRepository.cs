using Microsoft.EntityFrameworkCore;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product> GetByCodeAsync(string productCode)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }
    }
}