using FMCGEnterpriseManagementSystem.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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

        public Task<(bool Success, string Message)> EmailInvoiceAsync(int invoiceId, string recipientEmail)
            => SendAsync($"api/invoices/{invoiceId}/email", recipientEmail, invoiceId);

        public Task<(bool Success, string Message)> EmailQuoteAsync(int quoteId, string recipientEmail)
            => SendAsync($"api/quotes/{quoteId}/email", recipientEmail, quoteId);

        public Task<(bool Success, string Message)> EmailPaymentAsync(int paymentId, string recipientEmail)
            => SendAsync($"api/payments/{paymentId}/email", recipientEmail, paymentId);

        private async Task<(bool Success, string Message)> SendAsync(string endpoint, string recipientEmail, int recordId)
        {
            try
            {
                var payload = new { RecordId = recordId, RecipientEmail = recipientEmail };
                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Email sent successfully.");
                }

                var errorBody = await response.Content.ReadAsStringAsync();
                return (false, $"API returned {(int)response.StatusCode}: {errorBody}");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to reach the email API: {ex.Message}");
            }
        }
    }
}