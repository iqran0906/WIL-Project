// Title: DataAnnotations Validation Attributes
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model used to collect the information required
    // when a user signs into the application.
    public class LoginViewModel
    {
        // Email address used to identify the user's account.
        // Required prevents an empty submission.
        // EmailAddress validates the email format.
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // Password entered by the user during authentication.
        // DataType.Password allows the MVC view to render
        // the field as a password input.
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Determines whether the user's login should be remembered
        // by the authentication process.
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
