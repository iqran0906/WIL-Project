// Purpose: Login account (ASP.NET Core Identity user) with an active/inactive flag.
// Authors: Naseeha27 (from git history)
// Uses: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}