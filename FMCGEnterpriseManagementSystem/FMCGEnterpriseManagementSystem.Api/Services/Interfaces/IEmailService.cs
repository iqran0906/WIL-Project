// Purpose: Defines email operations used by the FMCG Enterprise Management System.
// Authors: Sayali-St10458649, iqran0906
// Uses: System.Net.Mail (Microsoft, .NET base class library)
// https://learn.microsoft.com/dotnet/api/system.net.mail

using FMCGEnterpriseManagementSystem.Api.DTOs;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResultDto> SendInvoiceEmailAsync(
            EmailRequestDto request);

        Task<EmailResultDto> SendQuoteEmailAsync(
            EmailRequestDto request);

        Task<EmailResultDto> SendPaymentEmailAsync(
            EmailRequestDto request);

        Task<EmailResultDto> SendPasswordResetEmailAsync(
            PasswordResetEmailRequestDto request);
    }
}