// Title: System.ComponentModel.DataAnnotations Namespace
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model used to create, edit and display sales representative information.
    // Data annotation attributes provide validation and display metadata for MVC forms.
    public class SalesRepresentativeViewModel
    {
        // Unique identifier of the sales representative record.
        public int SalesRepresentativeId { get; set; }

        // Employee associated with the sales representative.
        [Required(ErrorMessage = "Please select an employee.")]
        [Display(Name = "Employee")]
        public string EmployeeID { get; set; } = string.Empty;

        // Employee number displayed to users.
        [Display(Name = "Employee Number")]
        public string? EmployeeNumber { get; set; }

        // Employee's display name.
        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }

        // Email address associated with the employee.
        public string? Email { get; set; }

        // Unique code used to identify the sales representative.
        [StringLength(20)]
        [Display(Name = "Sales Rep Code")]
        public string SalesRepCode { get; set; } = string.Empty;

        // Sales area or geographical region assigned to the representative.
        [StringLength(100)]
        public string? Area { get; set; }

        // Base salary of the sales representative.
        [Range(0, 999999999.99,
            ErrorMessage = "Salary cannot be negative.")]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        // Percentage commission earned on applicable sales.
        [Range(0, 100,
            ErrorMessage = "Commission rate must be between 0 and 100.")]
        [Display(Name = "Commission Rate (%)")]
        public decimal CommissionRate { get; set; }

        // Sales target assigned to the representative.
        [Range(0, 999999999.99,
            ErrorMessage = "Sales target cannot be negative.")]
        [Display(Name = "Sales Target")]
        public decimal SalesTarget { get; set; }

        // Indicates whether the sales representative is currently active.
        public bool IsActive { get; set; } = true;
    }
}
