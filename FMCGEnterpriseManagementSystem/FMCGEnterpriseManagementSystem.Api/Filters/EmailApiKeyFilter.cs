using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

namespace FMCGEnterpriseManagementSystem.Api.Filters
{
    // Verifies that requests originate from a caller possessing the configured email API key.
    public class EmailApiKeyFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;

        public EmailApiKeyFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var configuredKey = _configuration["EmailApi:ApiKey"];

            // Do not allow email requests if the key is missing.
            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                context.Result = new StatusCodeResult(
                    StatusCodes.Status503ServiceUnavailable);
                return;
            }

            // Check whether the request contains the API key.
            if (!context.HttpContext.Request.Headers.TryGetValue(
                    "X-Email-Api-Key", out var suppliedKey))
            {
                context.Result = new UnauthorizedObjectResult(
                    new { message = "API authentication is required." });
                return;
            }

            var expectedBytes = Encoding.UTF8.GetBytes(configuredKey);
            var suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey.ToString());

            // Compare keys securely.
            if (expectedBytes.Length != suppliedBytes.Length ||
                !CryptographicOperations.FixedTimeEquals(
                    expectedBytes, suppliedBytes))
            {
                context.Result = new UnauthorizedObjectResult(
                    new { message = "Invalid API credentials." });
                return;
            }

            await next();
        }
    }
}