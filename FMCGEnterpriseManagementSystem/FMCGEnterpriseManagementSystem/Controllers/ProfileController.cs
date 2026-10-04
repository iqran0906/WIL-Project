// Title: ASP.NET Core Identity
// Author: Microsoft
// Date: 10-11-2025
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/security/authentication/identity
// 
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts profile management to authenticated users.
    // Every action works on the currently logged-in user only.
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        // Identity managers and the database context are provided through dependency injection.
        public ProfileController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Profile/Edit
        // Loads the profile belonging to the currently authenticated user.
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var employee = await GetEmployeeAsync(user.Id);

            return View(new EditProfileViewModel
            {
                HasEmployeeRecord = employee != null,
                FirstName = employee?.FirstName,
                LastName = employee?.LastName,
                Email = user.Email ?? string.Empty,
                ContactNumber = employee?.ContactNumber ?? user.PhoneNumber
            });
        }

        // POST: Profile/Edit
        // Updates the current user's Identity and employee details.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // A tracked employee entity is required because the record may be updated.
            var employee = await GetEmployeeAsync(user.Id, tracked: true);
            model.HasEmployeeRecord = employee != null;

            // Employees must keep a name and contact number on file.
            if (employee != null)
            {
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    ModelState.AddModelError(nameof(model.FirstName), "Name is required.");

                if (string.IsNullOrWhiteSpace(model.LastName))
                    ModelState.AddModelError(nameof(model.LastName), "Surname is required.");

                if (string.IsNullOrWhiteSpace(model.ContactNumber))
                    ModelState.AddModelError(nameof(model.ContactNumber), "Contact number is required.");
            }

            var email = model.Email.Trim();
            var emailChanged = !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase);

            // The email is also the login, so it must not belong to another user or employee.
            if (emailChanged)
            {
                var existingUser = await _userManager.FindByEmailAsync(email);

                var usedByEmployee = await _context.Employees.AnyAsync(e =>
                    e.Email == email &&
                    (employee == null || e.EmployeeID != employee.EmployeeID));

                if ((existingUser != null && existingUser.Id != user.Id) || usedByEmployee)
                {
                    ModelState.AddModelError(nameof(model.Email), "This email address is already in use.");
                }
            }

            // Return the form with validation errors instead of saving invalid information.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Keeps Identity and employee updates within the same database transaction.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // Update Identity login details when the email address changes.
            if (emailChanged)
            {
                user.Email = email;
                user.UserName = email;
            }

            user.PhoneNumber = model.ContactNumber?.Trim();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            // Update the related employee record when one exists.
            if (employee != null)
            {
                employee.FirstName = model.FirstName!.Trim();
                employee.LastName = model.LastName!.Trim();
                employee.Email = email;
                employee.ContactNumber = model.ContactNumber!.Trim();
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            // Refreshes the authentication cookie so updated login details take effect immediately.
            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Your details have been updated.";

            return RedirectToAction("UserProfile", "Home");
        }

        // GET: Profile/ChangePassword
        // Displays the password-change form.
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // POST: Profile/ChangePassword
        // Changes the password for the currently authenticated user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // ASP.NET Core Identity validates the current password and applies the new password.
            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    var field = error.Code == "PasswordMismatch"
                        ? nameof(model.CurrentPassword)
                        : nameof(model.NewPassword);

                    var message = error.Code == "PasswordMismatch"
                        ? "Your current password is incorrect."
                        : error.Description;

                    ModelState.AddModelError(field, message);
                }

                return View(model);
            }

            // Keeps the user signed in after the password has been changed.
            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Your password has been changed.";

            return RedirectToAction("UserProfile", "Home");
        }

        // Retrieves the employee record linked to the current Identity user. No request-supplied employee ID is used.
        private async Task<Employee?> GetEmployeeAsync(string userId, bool tracked = false)
        {
            var employees = tracked
                ? _context.Employees
                : _context.Employees.AsNoTracking();

            return await employees.FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}