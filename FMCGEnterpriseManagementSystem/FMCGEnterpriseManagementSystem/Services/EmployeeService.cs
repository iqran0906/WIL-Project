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

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            if (await _employeeRepository.EmployeeNumberExistsAsync(employee.EmployeeNumber))
            {
                return false;
            }

            if (await _employeeRepository.EmailExistsAsync(employee.Email))
            {
                return false;
            }

            employee.EmployeeID =
                "EMP-" + Guid.NewGuid().ToString("N")[..16];

            employee.IsActive = true;
            employee.CreatedAt = DateTime.UtcNow;

            if (employee.NextOfKin == null)
            {
                return false;
            }

            employee.NextOfKin.NextOfKinID =
                "NOK-" + Guid.NewGuid().ToString("N")[..16];

            employee.NextOfKin.EmployeeID = employee.EmployeeID;

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

            if (await _employeeRepository.EmployeeNumberExistsAsync(
                    employee.EmployeeNumber,
                    employee.EmployeeID))
            {
                return false;
            }

            if (await _employeeRepository.EmailExistsAsync(
                    employee.Email,
                    employee.EmployeeID))
            {
                return false;
            }

            existingEmployee.EmployeeNumber = employee.EmployeeNumber;
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.ContactNumber = employee.ContactNumber;
            existingEmployee.JobTitle = employee.JobTitle;
            existingEmployee.DateOfEmployment = employee.DateOfEmployment;
           

            if (employee.NextOfKin != null)
            {
                if (existingEmployee.NextOfKin == null)
                {
                    existingEmployee.NextOfKin = new NextOfKin
                    {
                        NextOfKinID = "NOK-" + Guid.NewGuid().ToString("N")[..16],
                        EmployeeID = existingEmployee.EmployeeID
                    };
                }

                existingEmployee.NextOfKin.FullName =
                    employee.NextOfKin.FullName;

                existingEmployee.NextOfKin.Relationship =
                    employee.NextOfKin.Relationship;

                existingEmployee.NextOfKin.ContactNumber =
                    employee.NextOfKin.ContactNumber;

                existingEmployee.NextOfKin.Email =
                    employee.NextOfKin.Email;
            }
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