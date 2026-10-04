using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

/*****************************
*    Title: System.Net.Mail Namespace
*    Author: Microsoft
*    Date: 2024
*    Code version: .NET 10
*    Availability: https://learn.microsoft.com/dotnet/api/system.net.mail
******************************/

namespace FMCGEnterpriseManagementSystem.Api.Services
{
    // Handles sending invoices, quotes, payment confirmations
    // and password reset emails.
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        // Gets email configuration settings from the application configuration.
        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        // =========================
        // INVOICE EMAIL
        // =========================

        // Sends an invoice to the customer's email address.
        public async Task<EmailResultDto> SendInvoiceEmailAsync(
            EmailRequestDto request)
        {
            var subject =
                $"Your Invoice #{request.RecordId}";

            var body =
                $"Dear Customer,\n\n" +
                $"Please find attached your invoice " +
                $"(Ref: {request.RecordId}).\n\n" +
                $"Thank you for your business.";

            return await SendEmailAsync(
                request,
                subject,
                body);
        }


        // =========================
        // QUOTE EMAIL
        // =========================

        // Sends a quote to the customer's email address.
        public async Task<EmailResultDto> SendQuoteEmailAsync(
            EmailRequestDto request)
        {
            var subject =
                $"Your Quote #{request.RecordId}";

            var body =
                $"Dear Customer,\n\n" +
                $"Please find attached your quote " +
                $"(Ref: {request.RecordId}).\n\n" +
                $"We look forward to your response.";

            return await SendEmailAsync(
                request,
                subject,
                body);
        }


        // =========================
        // PAYMENT EMAIL
        // =========================

        // Sends a payment confirmation to the customer's email address.
        public async Task<EmailResultDto> SendPaymentEmailAsync(
            EmailRequestDto request)
        {
            var subject =
                $"Payment Confirmation #{request.RecordId}";

            var body =
                $"Dear Customer,\n\n" +
                $"This confirms we have received your payment " +
                $"(Ref: {request.RecordId}).\n\n" +
                $"Thank you.";

            return await SendEmailAsync(
                request,
                subject,
                body);
        }


        // =========================
        // PASSWORD RESET EMAIL
        // =========================

        // Sends a secure password reset link to a system user.
        public async Task<EmailResultDto> SendPasswordResetEmailAsync(
            PasswordResetEmailRequestDto request)
        {
            var subject =
                "Exclusive Distributors - Password Reset";

            var body =
                "Dear User,\n\n" +
                "A password reset was requested for your " +
                "Exclusive Distributors account.\n\n" +
                "Use the link below to create a new password:\n\n" +
                request.ResetUrl +
                "\n\n" +
                "If you did not request a password reset, " +
                "you can safely ignore this email.\n\n" +
                "Exclusive Distributors";

            return await SendPasswordResetEmailInternalAsync(
                request,
                subject,
                body);
        }


        // =========================
        // STANDARD EMAIL SENDER
        // =========================

        // Creates and sends invoice, quote and payment emails.
        // These emails may contain a PDF attachment.
        private async Task<EmailResultDto> SendEmailAsync(
            EmailRequestDto request,
            string subject,
            string body)
        {
            try
            {
                var smtpHost =
                    _config["EmailSettings:SmtpHost"];

                var smtpPort =
                    int.Parse(
                        _config["EmailSettings:SmtpPort"]!);

                var senderEmail =
                    _config["EmailSettings:SenderEmail"];

                var senderPassword =
                    _config["EmailSettings:SenderPassword"];


                if (string.IsNullOrWhiteSpace(smtpHost) ||
                    string.IsNullOrWhiteSpace(senderEmail) ||
                    string.IsNullOrWhiteSpace(senderPassword))
                {
                    return new EmailResultDto
                    {
                        Success = false,
                        Message =
                            "Email configuration is incomplete."
                    };
                }


                using var client =
                    new SmtpClient(
                        smtpHost,
                        smtpPort)
                    {
                        Credentials =
                            new NetworkCredential(
                                senderEmail,
                                senderPassword),

                        EnableSsl = true
                    };


                using var mailMessage =
                    new MailMessage(
                        senderEmail,
                        request.RecipientEmail,
                        subject,
                        body);


                if (request.AttachmentBytes != null &&
                    request.AttachmentBytes.Length > 0)
                {
                    var stream =
                        new MemoryStream(
                            request.AttachmentBytes);

                    var fileName =
                        string.IsNullOrWhiteSpace(
                            request.AttachmentFileName)
                            ? "document.pdf"
                            : request.AttachmentFileName;


                    mailMessage.Attachments.Add(
                        new Attachment(
                            stream,
                            fileName,
                            "application/pdf"));
                }


                await client.SendMailAsync(
                    mailMessage);


                return new EmailResultDto
                {
                    Success = true,
                    Message =
                        "Email sent successfully."
                };
            }
            catch (Exception ex)
            {
                return new EmailResultDto
                {
                    Success = false,
                    Message =
                        $"Failed to send email: {ex.Message}"
                };
            }
        }


        // =========================
        // PASSWORD RESET SENDER
        // =========================

        // Sends a password reset email without a PDF attachment.
        private async Task<EmailResultDto>
            SendPasswordResetEmailInternalAsync(
                PasswordResetEmailRequestDto request,
                string subject,
                string body)
        {
            try
            {
                var smtpHost =
                    _config["EmailSettings:SmtpHost"];

                var smtpPort =
                    int.Parse(
                        _config["EmailSettings:SmtpPort"]!);

                var senderEmail =
                    _config["EmailSettings:SenderEmail"];

                var senderPassword =
                    _config["EmailSettings:SenderPassword"];


                if (string.IsNullOrWhiteSpace(smtpHost) ||
                    string.IsNullOrWhiteSpace(senderEmail) ||
                    string.IsNullOrWhiteSpace(senderPassword))
                {
                    return new EmailResultDto
                    {
                        Success = false,
                        Message =
                            "Email configuration is incomplete."
                    };
                }


                using var client =
                    new SmtpClient(
                        smtpHost,
                        smtpPort)
                    {
                        Credentials =
                            new NetworkCredential(
                                senderEmail,
                                senderPassword),

                        EnableSsl = true
                    };


                using var mailMessage =
                    new MailMessage(
                        senderEmail,
                        request.RecipientEmail,
                        subject,
                        body);


                await client.SendMailAsync(
                    mailMessage);


                return new EmailResultDto
                {
                    Success = true,
                    Message =
                        "Password reset email sent successfully."
                };
            }
            catch (Exception ex)
            {
                return new EmailResultDto
                {
                    Success = false,
                    Message =
                        $"Failed to send password reset email: " +
                        $"{ex.Message}"
                };
            }
        }
    }
}