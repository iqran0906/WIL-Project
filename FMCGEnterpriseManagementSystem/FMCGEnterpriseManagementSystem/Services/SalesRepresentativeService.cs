using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class SalesRepresentativeService
        : ISalesRepresentativeService
    {
        private readonly ISalesRepresentativeRepository
            _salesRepresentativeRepository;

        public SalesRepresentativeService(
            ISalesRepresentativeRepository
                salesRepresentativeRepository)
        {
            _salesRepresentativeRepository =
                salesRepresentativeRepository;
        }

        public async Task<IEnumerable<SalesRepresentativeViewModel>>
            GetAllAsync(string? keyword = null)
        {
            var salesRepresentatives =
                await _salesRepresentativeRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var search = keyword.Trim();

                salesRepresentatives =
                    salesRepresentatives.Where(sr =>
                        sr.SalesRepCode.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        (sr.Area != null &&
                         sr.Area.Contains(
                             search,
                             StringComparison.OrdinalIgnoreCase)) ||

                        sr.Employee.EmployeeNumber.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        sr.Employee.FirstName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        sr.Employee.LastName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        sr.Employee.Email.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase));
            }

            return salesRepresentatives
                .Select(MapToViewModel)
                .ToList();
        }

        public async Task<SalesRepresentativeViewModel?> GetByIdAsync(
            int salesRepresentativeId)
        {
            var salesRepresentative =
                await _salesRepresentativeRepository.GetByIdAsync(
                    salesRepresentativeId);

            if (salesRepresentative == null)
            {
                return null;
            }

            return MapToViewModel(salesRepresentative);
        }

        public async Task<IEnumerable<Employee>>
            GetEligibleEmployeesAsync()
        {
            return await _salesRepresentativeRepository
                .GetEligibleEmployeesAsync();
        }

        public async Task<bool> CreateAsync(
            SalesRepresentativeViewModel model)
        {
            var existingEmployeeSalesRep =
                await _salesRepresentativeRepository
                    .GetByEmployeeIdAsync(model.EmployeeID);

            if (existingEmployeeSalesRep != null)
            {
                return false;
            }

            var codeExists =
                await _salesRepresentativeRepository
                    .SalesRepCodeExistsAsync(
                        model.SalesRepCode.Trim());

            if (codeExists)
            {
                return false;
            }

            var salesRepresentative =
                new SalesRepresentative
                {
                    EmployeeID = model.EmployeeID,
                    SalesRepCode = model.SalesRepCode.Trim(),
                    Area = string.IsNullOrWhiteSpace(model.Area)
                        ? null
                        : model.Area.Trim(),
                    Salary = model.Salary,
                    CommissionRate = model.CommissionRate,
                    SalesTarget = model.SalesTarget,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

            await _salesRepresentativeRepository
                .AddAsync(salesRepresentative);

            await _salesRepresentativeRepository
                .SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(
            SalesRepresentativeViewModel model)
        {
            var salesRepresentative =
                await _salesRepresentativeRepository.GetByIdAsync(
                    model.SalesRepresentativeId);

            if (salesRepresentative == null)
            {
                return false;
            }

            var codeExists =
                await _salesRepresentativeRepository
                    .SalesRepCodeExistsAsync(
                        model.SalesRepCode.Trim(),
                        model.SalesRepresentativeId);

            if (codeExists)
            {
                return false;
            }

            salesRepresentative.SalesRepCode =
                model.SalesRepCode.Trim();

            salesRepresentative.Area =
                string.IsNullOrWhiteSpace(model.Area)
                    ? null
                    : model.Area.Trim();

            salesRepresentative.Salary =
                model.Salary;

            salesRepresentative.CommissionRate =
                model.CommissionRate;

            salesRepresentative.SalesTarget =
                model.SalesTarget;

            salesRepresentative.UpdatedAt =
                DateTime.UtcNow;

            _salesRepresentativeRepository
                .Update(salesRepresentative);

            await _salesRepresentativeRepository
                .SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(
            int salesRepresentativeId)
        {
            var salesRepresentative =
                await _salesRepresentativeRepository.GetByIdAsync(
                    salesRepresentativeId);

            if (salesRepresentative == null)
            {
                return false;
            }

            if (!salesRepresentative.IsActive)
            {
                return true;
            }

            salesRepresentative.IsActive = false;
            salesRepresentative.UpdatedAt = DateTime.UtcNow;

            _salesRepresentativeRepository
                .Update(salesRepresentative);

            await _salesRepresentativeRepository
                .SaveChangesAsync();

            return true;
        }

        public async Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null)
        {
            if (string.IsNullOrWhiteSpace(salesRepCode))
            {
                return false;
            }

            return await _salesRepresentativeRepository
                .SalesRepCodeExistsAsync(
                    salesRepCode.Trim(),
                    excludeSalesRepresentativeId);
        }

        private static SalesRepresentativeViewModel MapToViewModel(
            SalesRepresentative salesRepresentative)
        {
            return new SalesRepresentativeViewModel
            {
                SalesRepresentativeId =
                    salesRepresentative.SalesRepresentativeId,

                EmployeeID =
                    salesRepresentative.EmployeeID,

                EmployeeNumber =
                    salesRepresentative.Employee.EmployeeNumber,

                EmployeeName =
                    $"{salesRepresentative.Employee.FirstName} " +
                    $"{salesRepresentative.Employee.LastName}",

                Email =
                    salesRepresentative.Employee.Email,

                SalesRepCode =
                    salesRepresentative.SalesRepCode,

                Area =
                    salesRepresentative.Area,

                Salary =
                    salesRepresentative.Salary,

                CommissionRate =
                    salesRepresentative.CommissionRate,

                SalesTarget =
                    salesRepresentative.SalesTarget,

                IsActive =
                    salesRepresentative.IsActive
            };
        }
    }
}