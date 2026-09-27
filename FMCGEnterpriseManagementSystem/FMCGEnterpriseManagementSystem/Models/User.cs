using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}