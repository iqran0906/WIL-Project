// Purpose: Contract (interface) for building and sending invoice, quote, and payment emails.
// Authors: Sayali-St10458649

namespace FMCGEnterpriseManagementSystem.Api.DTOs
{
    public class EmailResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}