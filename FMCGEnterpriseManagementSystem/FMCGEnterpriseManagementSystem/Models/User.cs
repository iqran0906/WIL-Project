namespace FMCGEnterpriseManagementSystem.Models
{
    public class User
    {
        public string UserID { get; set; }

        public string RoleID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}