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

        public async Task<SupplierViewModel?> GetSupplierByIdAsync(string id)
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
            var supplier = MapToEntity(model);
            await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task DeleteSupplierAsync(string id)
        {
            await _supplierRepository.DeleteAsync(id);
        }

        private static SupplierViewModel MapToViewModel(Supplier s) => new()
        {
            SupplierID = s.SupplierID,
            CompanyName = s.CompanyName,
            ContactPerson = s.ContactPerson,
            ContactNumber = s.ContactNumber,
            Email = s.Email,
            PhysicalAddress = s.PhysicalAddress,
            CreditLimit = s.CreditLimit,
            CreditTerms = s.CreditTerms,
            VATNumber = s.VATNumber,
            Notes = s.Notes
        };

        private static Supplier MapToEntity(SupplierViewModel vm) => new()
        {
            SupplierID = vm.SupplierID,
            CompanyName = vm.CompanyName,
            ContactPerson = vm.ContactPerson,
            ContactNumber = vm.ContactNumber,
            Email = vm.Email,
            PhysicalAddress = vm.PhysicalAddress,
            CreditLimit = vm.CreditLimit,
            CreditTerms = vm.CreditTerms,
            VATNumber = vm.VATNumber,
            Notes = vm.Notes
        };
    }
}