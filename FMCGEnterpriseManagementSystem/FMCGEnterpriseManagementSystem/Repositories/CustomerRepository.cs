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
    // Implements database operations for managing customers.
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        // Retrieves active customers only.
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
                .Where(c => c.IsActive)
                .ToListAsync();
        }


        // Retrieves soft-deleted customers only.
        public async Task<IEnumerable<Customer>> GetDeletedAsync()
        {
            return await _context.Customers
                .Include(c => c.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
                .Where(c => !c.IsActive)
                .ToListAsync();
        }


        // Retrieves a customer regardless of active status.
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    c => c.CustomerId == id);
        }


        // Adds a new active customer.
        public async Task AddAsync(Customer customer)
        {
            customer.IsActive = true;

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }


        // Updates an existing customer without changing
        // whether the customer is active or deleted.
        public async Task UpdateAsync(Customer customer)
        {
            var existingCustomer =
                await _context.Customers
                    .FirstOrDefaultAsync(
                        c => c.CustomerId ==
                             customer.CustomerId);

            if (existingCustomer == null)
            {
                return;
            }

            existingCustomer.Name = customer.Name;
            existingCustomer.Surname = customer.Surname;
            existingCustomer.IdNumber = customer.IdNumber;
            existingCustomer.TelephoneNumber =
                customer.TelephoneNumber;
            existingCustomer.CellNumber =
                customer.CellNumber;
            existingCustomer.Email = customer.Email;
            existingCustomer.PhysicalAddress =
                customer.PhysicalAddress;
            existingCustomer.DeliveryAddress =
                customer.DeliveryAddress;
            existingCustomer.CustomerGroup =
                customer.CustomerGroup;
            existingCustomer.PaymentTerms =
                customer.PaymentTerms;
            existingCustomer.PaymentMethod =
                customer.PaymentMethod;
            existingCustomer.Notes =
                customer.Notes;
            existingCustomer.SalesRepresentativeId =
                customer.SalesRepresentativeId;
            existingCustomer.VATNumber =
                customer.VATNumber;

            existingCustomer.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


        // Soft deletes the customer instead of removing
        // the database record.
        public async Task DeleteAsync(int id)
        {
            var customer =
                await _context.Customers
                    .FirstOrDefaultAsync(
                        c => c.CustomerId == id);

            if (customer == null)
            {
                return;
            }

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


        // Restores a previously soft-deleted customer.
        public async Task RestoreAsync(int id)
        {
            var customer =
                await _context.Customers
                    .FirstOrDefaultAsync(
                        c => c.CustomerId == id);

            if (customer == null)
            {
                return;
            }

            customer.IsActive = true;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}