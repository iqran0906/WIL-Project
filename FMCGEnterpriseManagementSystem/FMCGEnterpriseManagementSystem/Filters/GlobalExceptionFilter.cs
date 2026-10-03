/***************************************************************************************
*    Title: Global MVC Error Handling Filter
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Filters/GlobalErrorHandlingFilter.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Filters in ASP.NET Core MVC
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core MVC
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters
***************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Filters
{
    // Catches unhandled exceptions from form submissions (POST, etc.),
    // logs them, and sends the user back to the page they were on with a
    // friendly message instead of a crash page.
    //
    // Errors on normal page loads (GET) are left to the exception handler
    // middleware, which shows the Error page.
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public GlobalExceptionFilter(
            ILogger<GlobalExceptionFilter> logger,
            ITempDataDictionaryFactory tempDataFactory)
        {
            _logger = logger;
            _tempDataFactory = tempDataFactory;
        }

        public void OnException(ExceptionContext context)
        {
            var request = context.HttpContext.Request;

            _logger.LogError(
                context.Exception,
                "Unhandled error during {Method} {Path} for user {User}",
                request.Method,
                request.Path,
                context.HttpContext.User.Identity?.Name ?? "anonymous");

            if (HttpMethods.IsGet(request.Method))
            {
                return;
            }

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);

            tempData["ErrorMessage"] = context.Exception is DbUpdateException
                ? "Your changes could not be saved because they conflict with existing records " +
                  "(for example, the record is still linked to other data). Please check and try again."
                : "Something went wrong while processing your request. Please try again. " +
                  "If the problem continues, contact your administrator.";

            context.Result = new RedirectResult(GetReturnUrl(context.HttpContext));
            context.ExceptionHandled = true;
        }

        // Go back to the page the form was submitted from, if it is on this site
        private static string GetReturnUrl(HttpContext httpContext)
        {
            var referer = httpContext.Request.Headers.Referer.ToString();

            if (Uri.TryCreate(referer, UriKind.Absolute, out var uri) &&
                string.Equals(uri.Host, httpContext.Request.Host.Host, StringComparison.OrdinalIgnoreCase))
            {
                return uri.PathAndQuery;
            }

            return "/Home/Dashboard";
        }
    }
}
