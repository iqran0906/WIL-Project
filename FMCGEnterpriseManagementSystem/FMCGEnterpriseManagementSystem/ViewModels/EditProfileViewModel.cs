// Title: System.ComponentModel.DataAnnotations Namespace
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model containing the profile information that
    // a logged-in user is permitted to update.
    public class EditProfileViewModel
    {
        // Indicates whether the user's login account is linked
        // to an employee record.
        public bool HasEmployeeRecord { get; set; }

        // Name fields only apply when the login is linked to an employee.

        // Stores the employee's first name.
        // StringLength limits the maximum number of characters.
        // Display controls the label shown in the UI.
        [StringLength(50)]
        [Display(Name = "Name")]
        public string? FirstName { get; set; }

        // Stores the employee's surname.
        [StringLength(50)]
        [Display(Name = "Surname")]
        public string? LastName { get; set; }

        // Stores the user's email address.
        // Required ensures that an email value must be supplied.
        // EmailAddress validates the value against an email format.
        // StringLength limits the maximum length.
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // Stores the user's contact number.
        // Phone provides validation for a telephone number format.
        [Phone(ErrorMessage = "Please enter a valid contact number.")]
        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }
    }

    // View model used when a logged-in user wants to change
    // their account password.
    public class ChangePasswordViewModel
    {
        // Stores the user's existing password.
        // The Required attribute prevents an empty value.
        // DataType.Password allows MVC to render the value
        // as a password field.
        [Required(ErrorMessage = "Enter your current password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        // Stores the new password selected by the user.
        // The StringLength validation requires a minimum of
        // eight characters and allows up to 100 characters.
        [Required(ErrorMessage = "Enter a new password.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The new password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        // Stores the confirmation of the new password.
        // Compare ensures that it matches NewPassword.
        [Required(ErrorMessage = "Confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "The passwords do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}