// Purpose: Contract (interface) for the auth service.
// Authors: iqran0906 (from git history)

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IAuthService 
    {
        Task<bool> LoginAsync(string email, string password, bool rememberMe);

        Task LogoutAsync();

        Task<bool> IsUserActiveAsync(string email);
    }
}