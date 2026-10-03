
using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net;


namespace FMCGEnterpriseManagementSystem.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<EmailResultDto> SendInvoiceEmailAsync(EmailRequestDto request)
        {
            var subject = $"Your Invoice #{request.RecordId}";
            var body = $"Dear Customer,\n\nPlease find attached your invoice (Ref: {request.RecordId}).\n\nThank you for your business.";
            return await SendEmailAsync(request, subject, body);
        }

        public async Task<EmailResultDto> SendQuoteEmailAsync(EmailRequestDto request)
        {
            var subject = $"Your Quote #{request.RecordId}";
            var body = $"Dear Customer,\n\nPlease find attached your quote (Ref: {request.RecordId}).\n\nWe look forward to your response.";
            return await SendEmailAsync(request, subject, body);
        }

        public async Task<EmailResultDto> SendPaymentEmailAsync(EmailRequestDto request)
        {
            var subject = $"Payment Confirmation #{request.RecordId}";
            var body = $"Dear Customer,\n\nThis confirms we have received your payment (Ref: {request.RecordId}).\n\nThank you.";
            return await SendEmailAsync(request, subject, body);
        }

        private async Task<EmailResultDto> SendEmailAsync(EmailRequestDto request, string subject, string body)
        {
            try
            {
                var smtpHost = _config["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
                var senderEmail = _config["EmailSettings:SenderEmail"];
                var senderPassword = _config["EmailSettings:SenderPassword"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage(senderEmail, request.RecipientEmail, subject, body);

                if (request.AttachmentBytes != null && request.AttachmentBytes.Length > 0)
                {
                    var stream = new MemoryStream(request.AttachmentBytes);
                    var fileName = string.IsNullOrWhiteSpace(request.AttachmentFileName)
                        ? "document.pdf"
                        : request.AttachmentFileName;

                    mailMessage.Attachments.Add(new Attachment(stream, fileName, "application/pdf"));
                }

                await client.SendMailAsync(mailMessage);
                return new EmailResultDto { Success = true, Message = "Email sent successfully." };
            }
            catch (Exception ex)
            {
                return new EmailResultDto { Success = false, Message = $"Failed to send email: {ex.Message}" };
            }
        }
    }
}