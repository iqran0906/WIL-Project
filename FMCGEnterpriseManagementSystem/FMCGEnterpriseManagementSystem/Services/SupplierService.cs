// Purpose: Business logic for supplier.
// Authors: iqran0906, Maseeha17, Naseeha27 (from git history)

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return suppliers.Select(s => MapToViewModel(s));
        }

        public async Task<SupplierViewModel?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier == null ? null : MapToViewModel(supplier);
        }

        public async Task CreateSupplierAsync(SupplierViewModel model)
        {
            var supplier = MapToEntity(model);
            await _supplierRepository.AddAsync(supplier);
        }

        public async Task UpdateSupplierAsync(SupplierViewModel model)
        {
            var supplier = await _supplierRepository.GetByIdAsync(model.SupplierId);

            if (supplier == null)
            {
                return;
            }

            supplier.CompanyName = model.CompanyName;
            supplier.ContactPerson = model.ContactPerson;
            supplier.ContactNumber = model.ContactNumber;
            supplier.Email = model.Email;
            supplier.PhysicalAddress = model.PhysicalAddress;
            supplier.CreditLimit = model.CreditLimit;
            supplier.CreditTerms = model.CreditTerms;
            supplier.VATNumber = model.VATNumber;
            supplier.Notes = model.Notes ?? string.Empty;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            await _supplierRepository.DeleteAsync(id);
        }

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

        public async Task<bool> ActivateSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return false;
            }

            supplier.IsActive = true;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);

            return true;
        }

        public async Task<bool> DeactivateSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return false;
            }

            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);

            return true;
        }

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