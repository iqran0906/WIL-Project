// Title: Customize the Identity model
// Author: Microsoft
// Date: 08-11-2025
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model
//
using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Extends ASP.NET Core IdentityUser with application-specific user information.
    public class User : IdentityUser
    {
        // Indicates whether the user account is currently active.
        public bool IsActive { get; set; } = true;
    }
}