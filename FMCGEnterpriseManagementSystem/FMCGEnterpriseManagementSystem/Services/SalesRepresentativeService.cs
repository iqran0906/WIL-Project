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
    // Service responsible for sales representative business operations.
    public class SalesRepresentativeService
        : ISalesRepresentativeService
    {
        // Repository used to access and manage sales representative records.
        private readonly ISalesRepresentativeRepository
            _salesRepresentativeRepository;

        // Initialises the service with the sales representative repository.
        public SalesRepresentativeService(
            ISalesRepresentativeRepository
                salesRepresentativeRepository)
        {
            _salesRepresentativeRepository =
                salesRepresentativeRepository;
        }

        // Retrieves all sales representatives and optionally filters them by a search keyword.
        public async Task<IEnumerable<SalesRepresentativeViewModel>>
            GetAllAsync(string? keyword = null)
        {
            // Retrieves all sales representatives from the repository.
            var salesRepresentatives =
                await _salesRepresentativeRepository.GetAllAsync();

            // Applies filtering when a search keyword has been provided.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Removes unnecessary spaces from the search keyword.
                var search = keyword.Trim();

                // Searches using the sales representative code, area,
                // employee number, first name, last name, or email address.
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

            // Converts the entities into view models and returns the results as a list.
            return salesRepresentatives
                .Select(MapToViewModel)
                .ToList();
        }

        // Retrieves a sales representative using their unique ID.
        public async Task<SalesRepresentativeViewModel?> GetByIdAsync(
            int salesRepresentativeId)
        {
            // Retrieves the sales representative from the repository.
            var salesRepresentative =
                await _salesRepresentativeRepository.GetByIdAsync(
                    salesRepresentativeId);

            // Returns null when the sales representative does not exist.
            if (salesRepresentative == null)
            {
                return null;
            }

            // Converts the entity into a view model.
            return MapToViewModel(salesRepresentative);
        }

        // Retrieves employees who are eligible to become sales representatives.
        public async Task<IEnumerable<Employee>>
            GetEligibleEmployeesAsync()
        {
            return await _salesRepresentativeRepository
                .GetEligibleEmployeesAsync();
        }

        // Creates a new sales representative.
        public async Task<bool> CreateAsync(
     SalesRepresentativeViewModel model)
        {
            // Checks whether the selected employee is already assigned
            // to an existing sales representative.
            var existingEmployeeSalesRep =
                await _salesRepresentativeRepository
                    .GetByEmployeeIdAsync(model.EmployeeID);

            // Prevents an employee from being assigned twice.
            if (existingEmployeeSalesRep != null)
            {
                return false;
            }

            // Generate the next Sales Representative code automatically.
            // Example: SR-001, SR-002, SR-003...
            var allSalesRepresentatives =
                await _salesRepresentativeRepository.GetAllAsync();

            // Stores the highest numeric sales representative code found.
            var highestNumber = 0;

            // Checks all existing sales representative codes to find the highest number.
            foreach (var salesRep in allSalesRepresentatives)
            {
                // Ignores records without a sales representative code.
                if (string.IsNullOrWhiteSpace(salesRep.SalesRepCode))
                {
                    continue;
                }

                // Only processes codes using the SR- prefix.
                if (!salesRep.SalesRepCode.StartsWith(
                        "SR-",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Extracts the numeric portion of the sales representative code.
                var numberPart =
                    salesRep.SalesRepCode.Substring(3);

                // Updates the highest number when a larger valid number is found.
                if (int.TryParse(numberPart, out var number) &&
                    number > highestNumber)
                {
                    highestNumber = number;
                }
            }

            // Calculates the next available sales representative number.
            var nextNumber = highestNumber + 1;

            string generatedCode;

            // Generates a sales representative code until a unique code is found.
            do
            {
                generatedCode = $"SR-{nextNumber:D3}";

                var codeExists =
                    await _salesRepresentativeRepository
                        .SalesRepCodeExistsAsync(generatedCode);

                // Stops generating codes when a unique code is found.
                if (!codeExists)
                {
                    break;
                }

                nextNumber++;

            } while (true);

            // Creates the new sales representative entity.
            var salesRepresentative =
                new SalesRepresentative
                {
                    EmployeeID = model.EmployeeID,

                    SalesRepCode = generatedCode,

                    // Stores null when no sales area is provided.
                    Area = string.IsNullOrWhiteSpace(model.Area)
                        ? null
                        : model.Area.Trim(),

                    Salary = model.Salary,

                    CommissionRate = model.CommissionRate,

                    SalesTarget = model.SalesTarget,

                    // New sales representatives are active by default.
                    IsActive = true,

                    // Records when the sales representative was created.
                    CreatedAt = DateTime.UtcNow
                };

            // Adds the new sales representative to the repository.
            await _salesRepresentativeRepository
                .AddAsync(salesRepresentative);

            // Saves the changes to the database.
            await _salesRepresentativeRepository
                .SaveChangesAsync();

            return true;
        }

        // Updates an existing sales representative.
        public async Task<bool> UpdateAsync(
            SalesRepresentativeViewModel model)
        {
            // Retrieves the sales representative being updated.
            var salesRepresentative =
                await _salesRepresentativeRepository.GetByIdAsync(
                    model.SalesRepresentativeId);

            // Stops the update when the record cannot be found.
            if (salesRepresentative == null)
            {
                return false;
            }

            // Checks whether the requested sales representative code
            // is already used by another record.
            var codeExists =
                await _salesRepresentativeRepository
                    .SalesRepCodeExistsAsync(
                        model.SalesRepCode.Trim(),
                        model.SalesRepresentativeId);

            // Prevents duplicate sales representative codes.
            if (codeExists)
            {
                return false;
            }

            // Updates the sales representative information.
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

            // Records when the record was last updated.
            salesRepresentative.UpdatedAt =
                DateTime.UtcNow;

            // Marks the entity as updated in the repository.
            _salesRepresentativeRepository
                .Update(salesRepresentative);

            // Saves the updated information to the database.
            await _salesRepresentativeRepository
                .SaveChangesAsync();

            return true;
        }

        // Deactivates a sales representative without deleting their record.
        public async Task<bool> DeactivateAsync(
            int salesRepresentativeId)
        {
            // Retrieves the sales representative by ID.
            var salesRepresentative =
                await _salesRepresentativeRepository.GetByIdAsync(
                    salesRepresentativeId);

            // Stops the operation when the record cannot be found.
            if (salesRepresentative == null)
            {
                return false;
            }

            // If the sales representative is already inactive,
            // no additional update is required.
            if (!salesRepresentative.IsActive)
            {
                return true;
            }

            // Marks the sales representative as inactive.
            salesRepresentative.IsActive = false;

            // Records the date and time of the deactivation.
            salesRepresentative.UpdatedAt = DateTime.UtcNow;

            // Updates the record in the repository.
            _salesRepresentativeRepository
                .Update(salesRepresentative);

            // Saves the changes to the database.
            await _salesRepresentativeRepository
                .SaveChangesAsync();

            return true;
        }

        // Checks whether a sales representative code already exists.
        public async Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null)
        {
            // Treats an empty or whitespace-only code as unavailable for checking.
            if (string.IsNullOrWhiteSpace(salesRepCode))
            {
                return false;
            }

            // Checks the repository using the trimmed code.
            return await _salesRepresentativeRepository
                .SalesRepCodeExistsAsync(
                    salesRepCode.Trim(),
                    excludeSalesRepresentativeId);
        }

        // Converts a SalesRepresentative entity into a view model.
        private static SalesRepresentativeViewModel MapToViewModel(
            SalesRepresentative salesRepresentative)
        {
            return new SalesRepresentativeViewModel
            {
                SalesRepresentativeId =
                    salesRepresentative.SalesRepresentativeId,

                EmployeeID =
                    salesRepresentative.EmployeeID,

                // Retrieves the employee number from the related employee.
                EmployeeNumber =
                    salesRepresentative.Employee.EmployeeNumber,

                // Combines the employee's first and last names for display.
                EmployeeName =
                    $"{salesRepresentative.Employee.FirstName} " +
                    $"{salesRepresentative.Employee.LastName}",

                // Retrieves the employee's email address.
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