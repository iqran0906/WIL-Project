// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for employee-related business operations.
    public class EmployeeService : IEmployeeService
    {
        // Repository used to access and manage employee records.
        private readonly IEmployeeRepository _employeeRepository;

        // Initialises the employee service with the employee repository.
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // Retrieves all employees from the repository.
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        // Retrieves an employee using their employee ID.
        public async Task<Employee?> GetEmployeeByIdAsync(string id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        // Retrieves an employee using their employee number.
        public async Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber)
        {
            return await _employeeRepository.GetByEmployeeNumberAsync(employeeNumber);
        }

        // Searches for employees using a keyword.
        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string keyword)
        {
            // When no keyword is provided, return all employees.
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await _employeeRepository.GetAllAsync();
            }

            // Otherwise, perform a repository search using the supplied keyword.
            return await _employeeRepository.SearchAsync(keyword);
        }

        // Checks whether an employee number already exists.
        public async Task<bool> EmployeeNumberExistsAsync(
    string employeeNumber,
    string? excludeEmployeeId = null)
        {
            // Passes the employee number and optional employee ID to the repository.
            return await _employeeRepository.EmployeeNumberExistsAsync(
                employeeNumber,
                excludeEmployeeId);
        }

        // Checks whether an email address is already associated with an employee.
        public async Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null)
        {
            // Passes the email and optional employee ID to the repository.
            return await _employeeRepository.EmailExistsAsync(
                email,
                excludeEmployeeId);
        }

        // Creates a new employee and generates the required employee identifiers.
        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            // Prevents the creation of an employee using an email address that already exists.
            if (await _employeeRepository.EmailExistsAsync(employee.Email))
            {
                return false;
            }

            // Retrieves existing employees to determine the next employee number.
            var employees = await _employeeRepository.GetAllAsync();

            // Finds the highest existing numeric employee number using the EMP- format.
            var highestNumber = employees
                .Where(e => !string.IsNullOrWhiteSpace(e.EmployeeNumber)
                            && e.EmployeeNumber.StartsWith("EMP-"))
                .Select(e =>
                {
                    // Extracts the numeric portion from the employee number.
                    var numberPart = e.EmployeeNumber.Substring(4);

                    // Converts the numeric portion into an integer.
                    return int.TryParse(numberPart, out var number)
                        ? number
                        : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            // Generates the next employee number.
            var nextNumber = highestNumber + 1;
            var generatedEmployeeNumber = $"EMP-{nextNumber:D3}";

            // Continues generating employee numbers until a unique number is found.
            while (await _employeeRepository.EmployeeNumberExistsAsync(
                       generatedEmployeeNumber))
            {
                nextNumber++;
                generatedEmployeeNumber = $"EMP-{nextNumber:D3}";
            }

            // Assigns the generated employee number to the new employee.
            employee.EmployeeNumber = generatedEmployeeNumber;

            // Generates a unique employee ID.
            employee.EmployeeID =
                "EMP-" + Guid.NewGuid().ToString("N")[..16];

            // Marks the new employee as active.
            employee.IsActive = true;

            // Records the date and time the employee was created.
            employee.CreatedAt = DateTime.UtcNow;

            // A next-of-kin record is required before the employee can be created.
            if (employee.NextOfKin == null)
            {
                return false;
            }

            // Generates a unique ID for the next-of-kin record.
            employee.NextOfKin.NextOfKinID =
                "NOK-" + Guid.NewGuid().ToString("N")[..16];

            // Links the next-of-kin record to the newly created employee.
            employee.NextOfKin.EmployeeID = employee.EmployeeID;

            // Adds the employee to the repository and saves the changes.
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        // Updates an existing employee and their next-of-kin information.
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            // Retrieves the existing employee from the repository.
            var existingEmployee =
                await _employeeRepository.GetByIdAsync(employee.EmployeeID);

            // Stops the update if the employee does not exist.
            if (existingEmployee == null)
            {
                return false;
            }

            // Prevents the employee from using an email address already assigned
            // to another employee.
            if (await _employeeRepository.EmailExistsAsync(
                    employee.Email,
                    employee.EmployeeID))
            {
                return false;
            }

            // Updates the employee's main information.
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.ContactNumber = employee.ContactNumber;
            existingEmployee.JobTitle = employee.JobTitle;
            existingEmployee.DateOfEmployment = employee.DateOfEmployment;

            // Updates existing next-of-kin information when supplied.
            if (employee.NextOfKin != null)
            {
                // Creates a next-of-kin record if the employee does not currently have one.
                if (existingEmployee.NextOfKin == null)
                {
                    existingEmployee.NextOfKin = new NextOfKin
                    {
                        NextOfKinID = "NOK-" + Guid.NewGuid().ToString("N")[..16],
                        EmployeeID = existingEmployee.EmployeeID
                    };
                }

                // Updates the next-of-kin's personal information.
                existingEmployee.NextOfKin.FullName =
                    employee.NextOfKin.FullName;

                existingEmployee.NextOfKin.Relationship =
                    employee.NextOfKin.Relationship;

                existingEmployee.NextOfKin.ContactNumber =
                    employee.NextOfKin.ContactNumber;

                existingEmployee.NextOfKin.Email =
                    employee.NextOfKin.Email;
            }

            // Records when the employee information was last updated.
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            // Updates the employee through the repository and saves the changes.
            await _employeeRepository.UpdateAsync(existingEmployee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        // Deactivates an employee without deleting their record from the database.
        public async Task<bool> DeactivateEmployeeAsync(string id)
        {
            // Retrieves the employee using their ID.
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            // Stops the operation if the employee does not exist.
            if (employee == null)
            {
                return false;
            }

            // Marks the employee as inactive.
            employee.IsActive = false;

            // Records the date and time of the deactivation.
            employee.UpdatedAt = DateTime.UtcNow;

            // Updates the employee and saves the changes.
            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }
    }
}