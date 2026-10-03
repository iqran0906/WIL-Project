/***************************************************************************************
*    Title: Error View Model
*    Author: Naseeha27
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Models/ErrorViewModel.cs
***************************************************************************************/
namespace FMCGEnterpriseManagementSystem.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int StatusCode { get; set; } = 500;

        public string Title => StatusCode switch
        {
            400 => "Invalid Request",
            403 => "Access Denied",
            404 => "Page Not Found",
            _ => "Something Went Wrong"
        };

        public string Message => StatusCode switch
        {
            400 => "The request could not be processed. Please go back, refresh the page and try again.",
            403 => "You do not have permission to view this page.",
            404 => "The page or record you are looking for does not exist or may have been removed.",
            _ => "An unexpected error occurred while processing your request. " +
                 "The error has been logged. Please try again, or contact your administrator if it continues."
        };

        public string Icon => StatusCode switch
        {
            404 => "bi-search",
            403 => "bi-shield-lock",
            _ => "bi-exclamation-triangle"
        };
    }
}
