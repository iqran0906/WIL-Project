// Purpose: SMTP email settings read from configuration (appsettings / user secrets).
// Authors: Sayali-St10458649 (from git history)

namespace FMCGEnterpriseManagementSystem.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}