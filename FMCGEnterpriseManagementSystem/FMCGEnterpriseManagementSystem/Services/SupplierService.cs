// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Provides business logic for managing suppliers.
    // The service communicates with the supplier repository
    // instead of accessing the database directly.
    public class SupplierService : ISupplierService
    {
        // Repository used to perform supplier data operations.
        private readonly ISupplierRepository _supplierRepository;

        // Constructor receives the supplier repository through dependency injection.
        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        // Retrieves all suppliers and converts the database entities
        // into view models for presentation.
        public async Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync()
        {
            // Retrieves supplier entities from the repository.
            var suppliers = await _supplierRepository.GetAllAsync();

            // Converts each Supplier entity into a SupplierViewModel.
            return suppliers.Select(s => MapToViewModel(s));
        }

        // Retrieves one supplier using its ID.
        public async Task<SupplierViewModel?> GetSupplierByIdAsync(int id)
        {
            // Retrieves the supplier from the repository.
            var supplier = await _supplierRepository.GetByIdAsync(id);

            // Returns null if the supplier does not exist;
            // otherwise converts it to a view model.
            return supplier == null ? null : MapToViewModel(supplier);
        }

        // Creates a new supplier.
        public async Task CreateSupplierAsync(SupplierViewModel model)
        {
            // Converts the view model received from the UI
            // into a database entity.
            var supplier = MapToEntity(model);

            // Sends the new supplier to the repository for storage.
            await _supplierRepository.AddAsync(supplier);
        }

        // Updates an existing supplier.
        public async Task UpdateSupplierAsync(SupplierViewModel model)
        {
            // Retrieves the existing supplier from the repository.
            var supplier = await _supplierRepository.GetByIdAsync(model.SupplierId);

            // Stops if the supplier does not exist.
            if (supplier == null)
            {
                return;
            }

            // Updates the supplier's editable fields.
            supplier.CompanyName = model.CompanyName;
            supplier.ContactPerson = model.ContactPerson;
            supplier.ContactNumber = model.ContactNumber;
            supplier.Email = model.Email;
            supplier.PhysicalAddress = model.PhysicalAddress;
            supplier.CreditLimit = model.CreditLimit;
            supplier.CreditTerms = model.CreditTerms;
            supplier.VATNumber = model.VATNumber;
            supplier.Notes = model.Notes ?? string.Empty;

            // Records the time of the update.
            supplier.UpdatedAt = DateTime.UtcNow;

            // Saves the updated supplier through the repository.
            await _supplierRepository.UpdateAsync(supplier);
        }

        // Deletes a supplier using its ID.
        public async Task DeleteSupplierAsync(int id)
        {
            await _supplierRepository.DeleteAsync(id);
        }

        // Converts a Supplier entity into a SupplierViewModel.
        private static SupplierViewModel MapToViewModel(Supplier s) => new()
        {
            SupplierId = s.SupplierId,
            CompanyName = s.CompanyName,
            ContactPerson = s.ContactPerson,
            ContactNumber = s.ContactNumber,
            Email = s.Email,
            PhysicalAddress = s.PhysicalAddress,
            CreditLimit = s.CreditLimit,
            CreditTerms = s.CreditTerms,
            VATNumber = s.VATNumber,
            Notes = s.Notes,
            IsActive = s.IsActive
        };

        // Activates a supplier account.
        public async Task<bool> ActivateSupplierAsync(int id)
        {
            // Retrieves the supplier that needs to be activated.
            var supplier = await _supplierRepository.GetByIdAsync(id);

            // Returns false if the supplier cannot be found.
            if (supplier == null)
            {
                return false;
            }

            // Marks the supplier as active.
            supplier.IsActive = true;

            // Records the time of the update.
            supplier.UpdatedAt = DateTime.UtcNow;

            // Saves the changes.
            await _supplierRepository.UpdateAsync(supplier);

            return true;
        }

        // Deactivates a supplier account.
        public async Task<bool> DeactivateSupplierAsync(int id)
        {
            // Retrieves the supplier that needs to be deactivated.
            var supplier = await _supplierRepository.GetByIdAsync(id);

            // Returns false if the supplier does not exist.
            if (supplier == null)
            {
                return false;
            }

            // Marks the supplier as inactive.
            supplier.IsActive = false;

            // Records the time of the update.
            supplier.UpdatedAt = DateTime.UtcNow;

            // Saves the changes through the repository.
            await _supplierRepository.UpdateAsync(supplier);

            return true;
        }

        // Converts a SupplierViewModel into a Supplier database entity.
        private static Supplier MapToEntity(SupplierViewModel vm) => new()
        {
            SupplierId = vm.SupplierId,
            CompanyName = vm.CompanyName,
            ContactPerson = vm.ContactPerson,
            ContactNumber = vm.ContactNumber,
            Email = vm.Email,
            PhysicalAddress = vm.PhysicalAddress,
            CreditLimit = vm.CreditLimit,
            CreditTerms = vm.CreditTerms,
            VATNumber = vm.VATNumber,
            Notes = vm.Notes ?? string.Empty,
            IsActive = vm.IsActive,
        };
    }
}