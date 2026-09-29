// Purpose: Lets any logged-in user edit their own details and change their password.
// Authors: ST10068525 (new file, not yet committed)
// Uses: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Lets any logged-in user edit their own details.
    // Every action works on the current user only - never on an id from the request.
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var employee = await GetEmployeeAsync(user.Id, tracked: true);
            model.HasEmployeeRecord = employee != null;

            // Employees must keep a name and contact number on file
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

            // The email is also the login, so it must not belong to anyone else
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            // Login details (the email doubles as the username)
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

            // Employee record
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

            // Re-issue the login cookie so the new email/username take effect straight away
            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Your details have been updated.";

            return RedirectToAction("UserProfile", "Home");
        }

        // GET: Profile/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // POST: Profile/ChangePassword
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

            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Your password has been changed.";

            return RedirectToAction("UserProfile", "Home");
        }

        private async Task<Employee?> GetEmployeeAsync(string userId, bool tracked = false)
        {
            var employees = tracked
                ? _context.Employees
                : _context.Employees.AsNoTracking();

            return await employees.FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}
