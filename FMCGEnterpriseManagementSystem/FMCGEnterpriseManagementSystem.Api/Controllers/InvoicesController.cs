using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly FMCGEnterpriseManagementSystem.Services.Interfaces.IInvoiceService _invoiceService;
        private readonly FMCGEnterpriseManagementSystem.Services.Interfaces.IInvoiceExportService _invoiceExportService;

        public InvoicesController(IEmailService emailService, FMCGEnterpriseManagementSystem.Services.Interfaces.IInvoiceService invoiceService,
            FMCGEnterpriseManagementSystem.Services.Interfaces.IInvoiceExportService invoiceExportService)
        {
            _emailService = emailService;
            _invoiceService = invoiceService;
            _invoiceExportService = invoiceExportService;
        }



        [HttpGet("{id}/email-check")]
        public IActionResult CheckEmailEligibility(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid invoice id",
                    Detail = "Invoice id must be a positive number.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new { InvoiceId = id, CanEmail = true });
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

            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Invoice not found",
                    Detail = $"No invoice exists with id {id}.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            request.RecordId = id;
            request.AttachmentBytes = _invoiceExportService.GenerateInvoicePdf(invoice);
            request.AttachmentFileName = $"Invoice-{invoice.InvoiceNumber}.pdf";

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