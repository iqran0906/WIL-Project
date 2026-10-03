/***************************************************************************************
*    Title: Identity User Account
*    Author: Naseeha27
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Models/User.cs
***************************************************************************************/

using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}