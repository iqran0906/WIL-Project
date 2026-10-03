/***************************************************************************************
*    Title: Search Service Interface
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/ISearchService.cs
***************************************************************************************/

using System.Security.Claims;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ISearchService
    {
        // Searches every record type the user's role is allowed to open
        Task<GlobalSearchViewModel> SearchAsync(string query, ClaimsPrincipal user);
    }
}
