using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces.FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public InvoicesController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("{id}/email")]
        public async Task<IActionResult> EmailInvoice(int id, [FromBody] EmailRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RecipientEmail))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "A recipient email address is required.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            request.RecordId = id;

            var result = await _emailService.SendInvoiceEmailAsync(request);

            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new ProblemDetails
                {
                    Title = "Email delivery failed",
                    Detail = result.Message,
                    Status = StatusCodes.Status502BadGateway
                });
            }

            return Ok(result);
        }
    }
}