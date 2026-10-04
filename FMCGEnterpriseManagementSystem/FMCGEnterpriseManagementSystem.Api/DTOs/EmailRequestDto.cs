// Purpose: Data transfer object for an outgoing email request (recipient + optional PDF attachment).
// Authors: Sayali-st10458649
namespace FMCGEnterpriseManagementSystem.Api.DTOs
{
    public class EmailRequestDto
    {
        public int RecordId { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public byte[]? AttachmentBytes { get; set; }
        public string? AttachmentFileName { get; set; }
    }
}