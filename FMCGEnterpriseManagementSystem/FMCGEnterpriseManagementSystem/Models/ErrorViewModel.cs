// Title: View model for displaying application error information.
// Author: Microsoft
// Date: 21-09-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling

namespace FMCGEnterpriseManagementSystem.Models
{
    // Stores the information displayed when an application error occurs.
    public class ErrorViewModel
    {
        // Stores the unique request identifier used to trace an error.
        public string? RequestId { get; set; }

        // Determines whether the request ID should be displayed to the user.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Stores the HTTP status code associated with the error.
        public int StatusCode { get; set; } = 500;

        // Provides a user-friendly title based on the HTTP status code.
        public string Title => StatusCode switch
        {
            400 => "Invalid Request",
            403 => "Access Denied",
            404 => "Page Not Found",
            _ => "Something Went Wrong"
        };

        // Provides a user-friendly error message based on the HTTP status code.
        public string Message => StatusCode switch
        {
            400 => "The request could not be processed. Please go back, refresh the page and try again.",
            403 => "You do not have permission to view this page.",
            404 => "The page or record you are looking for does not exist or may have been removed.",
            _ => "An unexpected error occurred while processing your request. " +
                 "The error has been logged. Please try again, or contact your administrator if it continues."
        };

        // Selects a Bootstrap icon based on the type of error.
        public string Icon => StatusCode switch
        {
            404 => "bi-search",
            403 => "bi-shield-lock",
            _ => "bi-exclamation-triangle"
        };
    }
}