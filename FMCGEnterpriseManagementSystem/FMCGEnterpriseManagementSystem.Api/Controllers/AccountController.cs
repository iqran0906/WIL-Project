using FMCGEnterpriseManagementSystem.Api.DTOs;
using FMCGEnterpriseManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public AccountController(
            IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("password-reset-email")]
        public async Task<IActionResult> SendPasswordResetEmail(
            [FromBody] PasswordResetEmailRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _emailService.SendPasswordResetEmailAsync(
                    request);

            if (!result.Success)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new ProblemDetails
                    {
                        Title = "Email delivery failed",
                        Detail = result.Message,
                        Status =
                            StatusCodes.Status502BadGateway
                    });
            }

            return Ok(result);
        }
    }
}