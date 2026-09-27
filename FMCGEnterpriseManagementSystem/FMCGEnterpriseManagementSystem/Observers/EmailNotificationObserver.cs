using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    public class EmailNotificationObserver : INotificationObserver
    {
        private readonly EmailSettings _emailSettings;

        public EmailNotificationObserver(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task HandleAsync(NotificationDto notificationDto)
        {
            // Recipient logic: for now this sends to the default sender/admin inbox.
            // Swap in the customer/sales rep email once that lookup is wired in.
            var recipientEmail = _emailSettings.SenderEmail;

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
    }
}