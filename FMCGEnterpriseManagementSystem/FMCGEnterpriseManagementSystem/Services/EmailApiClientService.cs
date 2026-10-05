using FMCGEnterpriseManagementSystem.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class EmailApiClientService : IEmailApiClientService
    {
        private readonly HttpClient _httpClient;

        public EmailApiClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // INVOICE EMAIL
       
        public Task<(bool Success, string Message)> EmailInvoiceAsync( 
            int invoiceId,
            string recipientEmail,
            byte[]? attachmentBytes,
            string? attachmentFileName)
        {
            return SendAsync(
                $"api/invoices/{invoiceId}/email",
                recipientEmail,
                invoiceId,
                attachmentBytes,
                attachmentFileName);
        }

        // QUOTE EMAIL
       
        public Task<(bool Success, string Message)> EmailQuoteAsync(
            int quoteId,
            string recipientEmail,
            byte[]? attachmentBytes,
            string? attachmentFileName)
        {
            return SendAsync(
                $"api/quotes/{quoteId}/email",
                recipientEmail,
                quoteId,
                attachmentBytes,
                attachmentFileName);
        }


        // =========================
        // PAYMENT EMAIL
        // =========================

        public Task<(bool Success, string Message)> EmailPaymentAsync(
            int paymentId,
            string recipientEmail,
            byte[]? attachmentBytes,
            string? attachmentFileName)
        {
            return SendAsync(
                $"api/payments/{paymentId}/email",
                recipientEmail,
                paymentId,
                attachmentBytes,
                attachmentFileName);
        }


        // =========================
        // PASSWORD RESET EMAIL
        // =========================

        public async Task<(bool Success, string Message)>
            SendPasswordResetEmailAsync(
                string recipientEmail,
                string resetUrl)
        {
            try
            {
                var payload = new
                {
                    RecipientEmail = recipientEmail,
                    ResetUrl = resetUrl
                };

                var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/account/password-reset-email",
                        payload);

                if (response.IsSuccessStatusCode)
                {
                    return (
                        true,
                        "Password reset email sent successfully.");
                }

                var errorBody =
                    await response.Content.ReadAsStringAsync();

                return (
                    false,
                    $"API returned {(int)response.StatusCode}: {errorBody}");
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Failed to reach the email API: {ex.Message}");
            }
        }


        // =========================
        // STANDARD EMAIL REQUEST
        // =========================

        private async Task<(bool Success, string Message)> SendAsync(
            string endpoint,
            string recipientEmail,
            int recordId,
            byte[]? attachmentBytes,
            string? attachmentFileName)
        {
            try
            {
                var payload = new
                {
                    RecordId = recordId,
                    RecipientEmail = recipientEmail,
                    AttachmentBytes = attachmentBytes,
                    AttachmentFileName = attachmentFileName
                };

                var response =
                    await _httpClient.PostAsJsonAsync(
                        endpoint,
                        payload);

                if (response.IsSuccessStatusCode)
                {
                    return (
                        true,
                        "Email sent successfully.");
                }

                var errorBody =
                    await response.Content.ReadAsStringAsync();

                return (
                    false,
                    $"API returned {(int)response.StatusCode}: {errorBody}");
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Failed to reach the email API: {ex.Message}");
            }
        }
    }
}