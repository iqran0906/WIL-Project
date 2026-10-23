using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


/*****************************
*    Title: Handle errors in ASP.NET Core APIs
*    Author: Microsoft
*    Date: 2024
*    Code version: ASP.NET Core 10
*    Availability: https://learn.microsoft.com/aspnet/core/web-api/handle-errors
******************************/

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

        // POST: api/invoices/{id}/email
        // Sends an invoice PDF generated and supplied by the MVC application.
        //
        // Reference:
        // Microsoft (2026) Controller action return types in ASP.NET Core Web API.
        // https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types

        [HttpPost("{id}/email")]
        public async Task<IActionResult> EmailInvoice(
            int id,
            [FromBody] EmailRequestDto request)
        {
            // Validate the invoice ID.
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid invoice ID",
                    Detail = "Invoice ID must be a positive number.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Validate the recipient email.
            if (request == null ||
                string.IsNullOrWhiteSpace(request.RecipientEmail))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "A recipient email address is required.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Validate that the MVC application supplied a PDF attachment.
            if (request.AttachmentBytes == null ||
                request.AttachmentBytes.Length == 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Missing invoice attachment",
                    Detail = "A generated invoice PDF must be supplied.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Ensure the request refers to the invoice in the API route.
            if (request.RecordId != id)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invoice ID mismatch",
                    Detail = "The invoice ID in the request does not match the URL.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // The MVC application has already retrieved the invoice and generated the PDF using its invoice export service.
            // So the API does not need to query the invoice again.

            request.RecordId = id;



            // Send the supplied invoice PDF.
            var result =
                await _emailService.SendInvoiceEmailAsync(request);

            if (!result.Success)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new ProblemDetails
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


       