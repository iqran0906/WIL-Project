// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Purpose: Implements database operations for managing customers.
    public class CustomerRepository : ICustomerRepository
    {
        // Provides access to the application's database context.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all customers with their sales representative and employee details.
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
                .ToListAsync();
        }

        // Retrieves a customer by ID with their sales representative and employee details.
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        // Adds a new customer to the database and saves the changes.
        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        // Updates an existing customer and saves the changes.
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        // Finds and deletes a customer when the customer exists.
        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}