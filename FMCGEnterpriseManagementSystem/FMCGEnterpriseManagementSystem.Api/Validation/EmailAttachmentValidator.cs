using FMCGEnterpriseManagementSystem.Api.DTOs;

namespace FMCGEnterpriseManagementSystem.Api.Validation
{
    public static class EmailAttachmentValidator
    {
        // Maximum decoded PDF size: 10 MB.
        public const int MaxPdfBytes = 10 * 1024 * 1024;

        public static string? Validate(EmailRequestDto request)
        {
            if (request.AttachmentBytes == null ||
                request.AttachmentBytes.Length == 0)
            {
                return "A PDF attachment is required.";
            }

            if (request.AttachmentBytes.Length > MaxPdfBytes)
            {
                return "The PDF attachment must not exceed 10 MB.";
            }

            if (string.IsNullOrWhiteSpace(request.AttachmentFileName) ||
                !request.AttachmentFileName.EndsWith(
                    ".pdf",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "The attachment must have a PDF filename.";
            }

            // Check for the standard PDF file signature: %PDF-
            var bytes = request.AttachmentBytes;

            if (bytes.Length < 5 ||
                bytes[0] != 0x25 ||
                bytes[1] != 0x50 ||
                bytes[2] != 0x44 ||
                bytes[3] != 0x46 ||
                bytes[4] != 0x2D)
            {
                return "The attachment is not a recognised PDF file.";
            }

            return null;
        }
    }
}