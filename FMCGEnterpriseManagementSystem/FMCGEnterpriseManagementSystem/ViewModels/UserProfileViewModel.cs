// Title: Classes and Objects - C# Programming Guide
// Author: Microsoft
// Date: 2026
// Code version: C#
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel containing details of the currently logged-in user
    // for display on the user's profile page.
    public class UserProfileViewModel
    {
        // Stores the username of the currently logged-in user.
        public string UserName { get; set; } = string.Empty;

        // Stores the email address associated with the user account.
        public string Email { get; set; } = string.Empty;

        // Indicates whether the user account is currently active.
        public bool IsActive { get; set; }

        // Stores the roles assigned to the current user.
        public List<string> Roles { get; set; } = new();

        // Indicates whether the user has a linked employee record.
        public bool HasEmployeeRecord { get; set; }

        // Stores the first name from the linked employee record.
        public string? FirstName { get; set; }

        // Stores the last name from the linked employee record.
        public string? LastName { get; set; }

        // Stores the employee number from the linked employee record.
        public string? EmployeeNumber { get; set; }

        // Stores the employee's contact number.
        public string? ContactNumber { get; set; }

        // Stores the employee's job title.
        public string? JobTitle { get; set; }

        // Stores the employee's date of employment.
        public DateTime? DateOfEmployment { get; set; }

        // Stores the sales representative code when the employee
        // is linked to a sales representative record.
        public string? SalesRepCode { get; set; }

        // Stores the sales area assigned to the sales representative.
        public string? SalesArea { get; set; }

        // Determines the name that should be displayed for the user.
        // The employee's full name is used when an employee record exists;
        // otherwise, the username is displayed.
        public string DisplayName =>
            HasEmployeeRecord
                ? $"{FirstName} {LastName}".Trim()
                : UserName;

        // Generates initials from the first two parts of the display name.
        public string Initials
        {
            get
            {
                // Splits the display name into individual non-empty name parts.
                var parts = DisplayName
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Takes up to the first two name parts and converts their
                // first characters to uppercase.
                return string.Concat(parts.Take(2).Select(p => char.ToUpper(p[0])));
            }
        }
    }
}