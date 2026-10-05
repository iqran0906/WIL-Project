//    Title: Model Validation in ASP.NET Core MVC
//    Author: Microsoft
//    Date: 30-08-2024
//    Code version: ASP.NET Core
//    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel used when creating a user account for an employee.
    // Data annotation attributes provide validation rules for the submitted form.
    public class CreateUserAccountViewModel
    {
        // Identifies the employee for whom the account is being created.
        [Required]
        public string EmployeeID { get; set; } = string.Empty;

        // Stores the email address that will be associated with the user account.
        // The EmailAddress attribute validates that the supplied value
        // follows a valid email address format.
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // Stores the system role that will be assigned to the user.
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        // Stores the temporary password created for the new account.
        // The DataType attribute identifies this field as a password input.
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Temporary Password")]
        public string TemporaryPassword { get; set; } = string.Empty;

        // Stores the password confirmation entered by the administrator.
        // Compare ensures that the confirmation matches TemporaryPassword.
        [Required]
        [DataType(DataType.Password)]
        [Compare(
            nameof(TemporaryPassword),
            ErrorMessage = "The passwords do not match.")]
        [Display(Name = "Confirm Temporary Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}