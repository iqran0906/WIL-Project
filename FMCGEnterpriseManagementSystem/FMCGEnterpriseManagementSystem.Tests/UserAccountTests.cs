using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace FMCGEnterpriseManagementSystem.Tests
{
    public class UserAccountTests
    {
        private static UserManager<User> CreateUserManager(
            ApplicationDbContext context)
        {
            var userStore = new UserStore<User>(context);

            return new UserManager<User>(
                userStore,
                Options.Create(new IdentityOptions()),
                new PasswordHasher<User>(),
                new List<IUserValidator<User>>
                {
                    new UserValidator<User>()
                },
                new List<IPasswordValidator<User>>
                {
                    new PasswordValidator<User>()
                },
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                NullLogger<UserManager<User>>.Instance);
        }

        [Fact]
        public async Task ActiveEmployee_ShouldCreateUserAccount()
        {
            // Arrange
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            await using var context =
                new ApplicationDbContext(options);

            var employee = new Employee
            {
                EmployeeID = "EMP001",
                EmployeeNumber = "E001",
                FirstName = "Test",
                LastName = "Employee",
                Email = "employee@test.com",
                ContactNumber = "0123456789",
                JobTitle = "Employee",
                DateOfEmployment = DateTime.Today,
                IsActive = true
            };

            context.Employees.Add(employee);

            context.Roles.Add(new IdentityRole
            {
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            });

            await context.SaveChangesAsync();

            var userManager =
                CreateUserManager(context);

            var userAccountService =
                new UserAccountService(
                    context,
                    userManager);

            // Act
            var result =
                await userAccountService.CreateAccountAsync(
                    "EMP001",
                    "employee@test.com",
                    "Password123!",
                    "Employee");

            // Assert
            Assert.True(result);

            var updatedEmployee =
                await context.Employees
                    .FirstAsync(
                        e => e.EmployeeID == "EMP001");

            Assert.False(
                string.IsNullOrWhiteSpace(
                    updatedEmployee.UserId));

            var createdUser =
                await context.Users
                    .FirstAsync(
                        u => u.Email == "employee@test.com");

            Assert.True(createdUser.IsActive);

            Assert.Equal(
                "employee@test.com",
                createdUser.UserName);
        }

        [Fact]
        public async Task DuplicateAccount_ShouldBePrevented()
        {
            // Arrange
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            await using var context =
                new ApplicationDbContext(options);

            var existingUser = new User
            {
                Id = "existing-user-id",
                UserName = "existing@test.com",
                Email = "existing@test.com",
                EmailConfirmed = true,
                IsActive = true
            };

            var employee = new Employee
            {
                EmployeeID = "EMP002",
                EmployeeNumber = "E002",
                FirstName = "Existing",
                LastName = "Employee",
                Email = "existing@test.com",
                ContactNumber = "0123456789",
                JobTitle = "Employee",
                DateOfEmployment = DateTime.Today,
                IsActive = true,
                UserId = existingUser.Id,
                User = existingUser
            };

            context.Users.Add(existingUser);
            context.Employees.Add(employee);

            context.Roles.Add(new IdentityRole
            {
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            });

            await context.SaveChangesAsync();

            var userManager =
                CreateUserManager(context);

            var userAccountService =
                new UserAccountService(
                    context,
                    userManager);

            // Act
            var result =
                await userAccountService.CreateAccountAsync(
                    "EMP002",
                    "newaccount@test.com",
                    "Password123!",
                    "Employee");

            // Assert
            Assert.False(result);

            var userCount =
                await context.Users.CountAsync();

            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task SalesRepresentativeRole_ShouldRequireActiveSalesRepresentative()
        {
            // Arrange
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            await using var context =
                new ApplicationDbContext(options);

            var employee = new Employee
            {
                EmployeeID = "EMP003",
                EmployeeNumber = "E003",
                FirstName = "Sales",
                LastName = "Employee",
                Email = "sales@test.com",
                ContactNumber = "0123456789",
                JobTitle = "Sales Representative",
                DateOfEmployment = DateTime.Today,
                IsActive = true
            };

            context.Employees.Add(employee);

            // No active SalesRepresentative exists for EMP003.

            context.Roles.Add(new IdentityRole
            {
                Name = "SalesRepresentative",
                NormalizedName = "SALESREPRESENTATIVE"
            });

            await context.SaveChangesAsync();

            var userManager =
                CreateUserManager(context);

            var userAccountService =
                new UserAccountService(
                    context,
                    userManager);

            // Act
            var result =
                await userAccountService.CreateAccountAsync(
                    "EMP003",
                    "sales@test.com",
                    "Password123!",
                    "SalesRepresentative");

            // Assert
            Assert.False(result);

            var userCount =
                await context.Users.CountAsync();

            Assert.Equal(0, userCount);
        }

        [Fact]
        public async Task DeactivateAccount_ShouldMakeUserInactive()
        {
            // Arrange
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            await using var context =
                new ApplicationDbContext(options);

            var existingUser = new User
            {
                Id = "user-004",
                UserName = "active@test.com",
                Email = "active@test.com",
                EmailConfirmed = true,
                IsActive = true
            };

            var employee = new Employee
            {
                EmployeeID = "EMP004",
                EmployeeNumber = "E004",
                FirstName = "Active",
                LastName = "Employee",
                Email = "active@test.com",
                ContactNumber = "0123456789",
                JobTitle = "Employee",
                DateOfEmployment = DateTime.Today,
                IsActive = true,
                UserId = existingUser.Id,
                User = existingUser
            };

            context.Users.Add(existingUser);
            context.Employees.Add(employee);

            await context.SaveChangesAsync();

            var userManager =
                CreateUserManager(context);

            var userAccountService =
                new UserAccountService(
                    context,
                    userManager);

            // Act
            var result =
                await userAccountService
                    .DeactivateAccountAsync("EMP004");

            // Assert
            Assert.True(result);

            var deactivatedUser =
                await context.Users
                    .FirstAsync(
                        u => u.Id == "user-004");

            Assert.False(deactivatedUser.IsActive);
        }
    }
}