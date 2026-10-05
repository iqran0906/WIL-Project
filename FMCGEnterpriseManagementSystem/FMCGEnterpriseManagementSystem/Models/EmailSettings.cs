// Title: SMTP email settings model for configuring outgoing application emails.
// Author: Microsoft
// Date: 04-10-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient

namespace FMCGEnterpriseManagementSystem.Models
{
    // Stores the SMTP configuration required to send emails from the system.
    public class EmailSettings
    {
        // SMTP server used to send outgoing emails.
        public string SmtpServer { get; set; }

        // Port used to connect to the SMTP server.
        public int SmtpPort { get; set; }

        // Email address used as the sender.
        public string SenderEmail { get; set; }

        // Display name shown to recipients.
        public string SenderName { get; set; }

        // Username used to authenticate with the SMTP server.
        public string Username { get; set; }

        // Password used to authenticate with the SMTP server.
        public string Password { get; set; }

        // Determines whether SSL is enabled for the SMTP connection.
        public bool EnableSsl { get; set; }
    }
}