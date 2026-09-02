using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(string id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber)
        {
            return await _employeeRepository.GetByEmployeeNumberAsync(employeeNumber);
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee) // Prevents duplicate employee numbers and emails.
        {
            if (await _employeeRepository.EmployeeNumberExistsAsync(employee.EmployeeNumber))
            {
                return false;
            }

            if (await _employeeRepository.EmailExistsAsync(employee.Email))
            {
                return false;
            }

            employee.EmployeeID = Guid.NewGuid().ToString();
            employee.IsActive = true;
            employee.CreatedAt = DateTime.UtcNow;

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee =
                await _employeeRepository.GetByIdAsync(employee.EmployeeID);

            if (existingEmployee == null)
            {
                return false;
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.ContactNumber = employee.ContactNumber;
            existingEmployee.JobTitle = employee.JobTitle;
            existingEmployee.DateOfEmployment = employee.DateOfEmployment;
            existingEmployee.UserID = employee.UserID;
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            await _employeeRepository.UpdateAsync(existingEmployee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateEmployeeAsync(string id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return false;
            }

            employee.IsActive = false;
            employee.UpdatedAt = DateTime.UtcNow;

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }
    }
}