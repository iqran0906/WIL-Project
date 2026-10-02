// Purpose: Result of an export: the file bytes, file name and content type.
// Authors: Sayali-St10458649 (from git history)

namespace FMCGEnterpriseManagementSystem.DTOs
{
    public class ExportResultDto
    {
        public byte[] FileContents { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}