// Title: Dependency injection in ASP.NET Core
// Author: Microsoft
// Date: 18-09-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    // Observer responsible for sending email notifications.
    public class EmailNotificationObserver : INotificationObserver
    {
        // Stores SMTP email configuration from the application settings.
        private readonly EmailSettings _emailSettings;

        // Provides access to company notification settings.
        private readonly ISettingsService _settingsService;

        // Injects the email settings and settings service through dependency injection.
        public EmailNotificationObserver(
            IOptions<EmailSettings> emailSettings,
            ISettingsService settingsService)
        {
            _emailSettings = emailSettings.Value;
            _settingsService = settingsService;
        }

        // Processes a notification and sends it through the configured SMTP server.
        public async Task HandleAsync(NotificationDto notificationDto)
        {
            try
            {
                // Retrieves the current company notification settings.
                var settings = await _settingsService.GetAsync();

                // Administrators can switch email alerts off on the Settings page.
                if (!settings.EmailNotificationsEnabled)
                {
                    return;
                }

                // Uses the configured notification email or falls back to the default sender email.
                var recipientEmail = string.IsNullOrWhiteSpace(settings.NotificationEmail)
                    ? _emailSettings.SenderEmail
                    : settings.NotificationEmail;

                // Creates the email message using the notification information.
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    message.To.Add(recipientEmail);
                    message.Subject = notificationDto.Title;
                    message.Body = notificationDto.Message;
                    message.IsBodyHtml = false;

                    // Creates the SMTP client used to send the email.
                    using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        // Configures SMTP authentication and SSL.
                        client.Credentials = new NetworkCredential(
                            _emailSettings.Username,
                            _emailSettings.Password);

                        client.EnableSsl = _emailSettings.EnableSsl;

                        // Sends the email asynchronously.
                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception)
            {
                // Prevents email delivery errors from interrupting the main application process.
            }
        }
    }
}