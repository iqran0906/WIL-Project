// Purpose: Contract (interface) for the search service.
// Authors: ST10068525 (new file, not yet committed)

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
