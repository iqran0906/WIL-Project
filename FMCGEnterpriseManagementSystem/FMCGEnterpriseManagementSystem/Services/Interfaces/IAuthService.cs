namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IAuthService 
    {
        Task<bool> LoginAsync(string usernameOrEmail, string password, bool rememberMe);

        Task LogoutAsync();

        Task<bool> IsUserActiveAsync(string usernameOrEmail);
    }
}