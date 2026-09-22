namespace FMCGEnterpriseManagementSystem.Api.DTOs
{
    public class EmailRequestDto
    {
        public int RecordId { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
    }
}