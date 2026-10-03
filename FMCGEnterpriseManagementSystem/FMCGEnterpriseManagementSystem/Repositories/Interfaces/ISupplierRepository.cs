/***************************************************************************************
*    Title: Supplier Repository Interface
*    Author: Naseeha27, Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/ISupplierRepository.cs
***************************************************************************************/
using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();

        Task<Supplier?> GetByIdAsync(int id);

        Task AddAsync(Supplier supplier);

        Task UpdateAsync(Supplier supplier);

        Task DeleteAsync(int id);
    }
}