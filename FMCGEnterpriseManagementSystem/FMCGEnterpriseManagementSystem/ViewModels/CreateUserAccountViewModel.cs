using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class CreateUserAccountViewModel
    {
        [Required]
        public string EmployeeID { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Temporary Password")]
        public string TemporaryPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(
            nameof(TemporaryPassword),
            ErrorMessage = "The passwords do not match.")]
        [Display(Name = "Confirm Temporary Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}