using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
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
    [Route("api/quotes")]
    public class QuotesController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly FMCGEnterpriseManagementSystem.Services.Interfaces.IQuoteService _quoteService;

        public QuotesController(IEmailService emailService, FMCGEnterpriseManagementSystem.Services.Interfaces.IQuoteService quoteService)
        {
            _emailService = emailService;
            _quoteService = quoteService;

        }



        [HttpGet("{id}/email-check")]
        public IActionResult CheckEmailEligibility(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid quote id",
                    Detail = "Quote id must be a positive number.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new { QuoteId = id, CanEmail = true });
        }


        [HttpPost("{id}/email")]
        public async Task<IActionResult> EmailQuote(int id, [FromBody] EmailRequestDto request)
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

            var quote = await _quoteService.GetQuoteByIdAsync(id);
            if (quote == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Quote not found",
                    Detail = $"No quote exists with id {id}.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            /*****************************
            *    Title: QuestPDF
            *    Author: QuestPDF (Marcin Ziąbek)
            *    Date: 2024
            *    Code version: Community License
            *    Availability: https://www.questpdf.com
            ******************************/


            request.RecordId = id;
            request.AttachmentBytes = FMCGEnterpriseManagementSystem.Services.QuotePdfGenerator.Generate(quote);
            request.AttachmentFileName = $"Quote-{quote.QuoteNumber}.pdf";

            var result = await _emailService.SendQuoteEmailAsync(request);

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