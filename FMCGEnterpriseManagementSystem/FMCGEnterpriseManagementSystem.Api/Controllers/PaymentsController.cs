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
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly FMCGEnterpriseManagementSystem.Services.Interfaces.IPaymentService _paymentService;


        public PaymentsController(IEmailService emailService, FMCGEnterpriseManagementSystem.Services.Interfaces.IPaymentService paymentService)
        {
            _emailService = emailService;
            _paymentService = paymentService;

        }

        [HttpGet("{id}/email-check")]
        public IActionResult CheckEmailEligibility(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid payment id",
                    Detail = "Payment id must be a positive number.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new { PaymentId = id, CanEmail = true });
        }


        
        [HttpPost("{id}/email")]
        public async Task<IActionResult> EmailPayment(int id, [FromBody] EmailRequestDto request)
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

            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Payment not found",
                    Detail = $"No payment exists with id {id}.",
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
            request.AttachmentBytes = FMCGEnterpriseManagementSystem.Services.PaymentPdfGenerator.Generate(payment);
            request.AttachmentFileName = $"Payment-{id}.pdf";

            var result = await _emailService.SendPaymentEmailAsync(request);

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