// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the authentication operations used to manage user access.
    public interface IAuthService
    {
        // Authenticates a user using their email address and password.
        // The rememberMe parameter determines whether the login should be remembered.
        Task<bool> LoginAsync(string email, string password, bool rememberMe);

        // Logs the currently authenticated user out of the system.
        Task LogoutAsync();

        // Checks whether a user associated with the specified email address
        // is currently active and permitted to access the system.
        Task<bool> IsUserActiveAsync(string email);
    }
}