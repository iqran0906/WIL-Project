

/***************************************************************************************
*    Title: Dependency injection in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
***************************************************************************************/

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
    public class EmailNotificationObserver : INotificationObserver
    {
        private readonly EmailSettings _emailSettings;
        private readonly ISettingsService _settingsService;

        public EmailNotificationObserver(
            IOptions<EmailSettings> emailSettings,
            ISettingsService settingsService)
        {
            _emailSettings = emailSettings.Value;
            _settingsService = settingsService;
        }

            public async Task HandleAsync(NotificationDto notificationDto)
        {
            try
            {
                var settings = await _settingsService.GetAsync();

                // Administrators can switch email alerts off on the Settings page
                if (!settings.EmailNotificationsEnabled)
                {
                    return;
                }

                // Send to the address chosen in Settings, otherwise the default sender/admin inbox
                var recipientEmail = string.IsNullOrWhiteSpace(settings.NotificationEmail)
                    ? _emailSettings.SenderEmail
                    : settings.NotificationEmail;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    message.To.Add(recipientEmail);
                    message.Subject = notificationDto.Title;
                    message.Body = notificationDto.Message;
                    message.IsBodyHtml = false;

                    using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                        client.EnableSsl = _emailSettings.EnableSsl;

                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}