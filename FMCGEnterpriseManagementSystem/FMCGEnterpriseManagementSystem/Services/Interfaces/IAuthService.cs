namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IAuthService 
    {
        Task<bool> LoginAsync(string email, string password, bool rememberMe);

        Task LogoutAsync();

        Task<bool> IsUserActiveAsync(string email);
    }
}