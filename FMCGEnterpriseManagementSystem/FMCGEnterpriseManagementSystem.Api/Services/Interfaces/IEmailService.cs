// Purpose: Builds the email subject/body for each record type and sends it via SMTP, with an optional PDF attachment.
// Authors: Sayali-St10458649
// Uses: System.Net.Mail (Microsoft, .NET base class library) https://learn.microsoft.com/dotnet/api/system.net.mail

using FMCGEnterpriseManagementSystem.Api.DTOs;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResultDto> SendInvoiceEmailAsync(EmailRequestDto request);
        Task<EmailResultDto> SendQuoteEmailAsync(EmailRequestDto request);
        Task<EmailResultDto> SendPaymentEmailAsync(EmailRequestDto request);
    }
}