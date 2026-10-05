using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Api.DTOs
{
    public class PasswordResetEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        public string ResetUrl { get; set; } = string.Empty;
    }
}