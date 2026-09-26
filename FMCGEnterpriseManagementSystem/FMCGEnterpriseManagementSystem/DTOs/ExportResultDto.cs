namespace FMCGEnterpriseManagementSystem.DTOs
{
    public class ExportResultDto
    {
        public byte[] FileContents { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}