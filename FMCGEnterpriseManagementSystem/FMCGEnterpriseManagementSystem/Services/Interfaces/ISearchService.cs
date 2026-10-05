// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Security.Claims;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the contract for the global search service.
    public interface ISearchService
    {
        // Searches every record type the user's role is allowed to open.
        // ClaimsPrincipal is used to determine the user's identity and permissions.
        // The asynchronous method returns the matching search results in a view model.
        Task<GlobalSearchViewModel> SearchAsync(string query, ClaimsPrincipal user);
    }
}