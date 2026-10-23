using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Api.DTOs
{
    // Validates requests to email invoices, quotes and payments.
    public class EmailRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid record ID is required.")]
        public int RecordId { get; set; }

        [Required(ErrorMessage = "A recipient email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A PDF attachment is required.")]
        public byte[]? AttachmentBytes { get; set; }

        [Required(ErrorMessage = "An attachment filename is required.")]
        public string? AttachmentFileName { get; set; }
    }
}