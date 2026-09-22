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