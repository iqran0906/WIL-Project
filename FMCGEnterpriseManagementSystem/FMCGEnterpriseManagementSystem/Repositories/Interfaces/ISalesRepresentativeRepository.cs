using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface ISalesRepresentativeRepository
    {
        Task<IEnumerable<SalesRepresentative>> GetAllAsync();

        Task<SalesRepresentative?> GetByIdAsync(
            int salesRepresentativeId);

        Task<SalesRepresentative?> GetByEmployeeIdAsync(
            string employeeId);

        Task<SalesRepresentative?> GetByCodeAsync(
            string salesRepCode);

        Task<IEnumerable<Employee>> GetEligibleEmployeesAsync();

        Task AddAsync(
            SalesRepresentative salesRepresentative);

        void Update(
            SalesRepresentative salesRepresentative);

        Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null);

        Task SaveChangesAsync();
    }
}