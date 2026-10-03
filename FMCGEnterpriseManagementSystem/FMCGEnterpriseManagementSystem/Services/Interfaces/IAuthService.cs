/***************************************************************************************
*    Title: Authentication Service Interface
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IAuthService.cs
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IAuthService 
    {
        Task<bool> LoginAsync(string email, string password, bool rememberMe);

        Task LogoutAsync();

        Task<bool> IsUserActiveAsync(string email);
    }
}