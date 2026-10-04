// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories.Implementations
{
    // Purpose: Provides database queries used for inventory forecasting and demand analysis.
    public class ForecastingRepository : IForecastingRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public ForecastingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets items where current stock is at or below reorder level.
        public async Task<IEnumerable<Inventory>> GetLowStockForRestockForecastAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        // Gets all inventory records with active products for analytical predictions.
        public async Task<IEnumerable<Inventory>> GetInventoryForDemandAnalysisAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product != null && i.Product.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}