// Purpose: Details of the logged-in user shown on the User Profile page.
// Authors: ST10068525 (new file, not yet committed)

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // Details of the currently logged-in user for the profile page
    public class UserProfileViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new();

        // From the linked employee record (null if the login has none)
        public bool HasEmployeeRecord { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? ContactNumber { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? DateOfEmployment { get; set; }

        // From the linked sales representative record, if any
        public string? SalesRepCode { get; set; }
        public string? SalesArea { get; set; }

        public string DisplayName =>
            HasEmployeeRecord
                ? $"{FirstName} {LastName}".Trim()
                : UserName;

        public string Initials
        {
            get
            {
                var parts = DisplayName
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                return string.Concat(parts.Take(2).Select(p => char.ToUpper(p[0])));
            }
        }
    }
}
