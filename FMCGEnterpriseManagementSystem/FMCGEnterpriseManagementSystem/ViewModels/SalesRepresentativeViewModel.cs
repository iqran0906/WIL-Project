using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class SalesRepresentativeViewModel
    {
        public int SalesRepresentativeId { get; set; }

        [Required(ErrorMessage = "Please select an employee.")]
        [Display(Name = "Employee")]
        public string EmployeeID { get; set; } = string.Empty;

        [Display(Name = "Employee Number")]
        public string? EmployeeNumber { get; set; }

        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }

        public string? Email { get; set; }

        [Required(ErrorMessage = "Sales representative code is required.")]
        [StringLength(20)]
        [Display(Name = "Sales Rep Code")]
        public string SalesRepCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Area { get; set; }

        [Range(0, 999999999.99,
            ErrorMessage = "Salary cannot be negative.")]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        [Range(0, 100,
            ErrorMessage = "Commission rate must be between 0 and 100.")]
        [Display(Name = "Commission Rate (%)")]
        public decimal CommissionRate { get; set; }

        [Range(0, 999999999.99,
            ErrorMessage = "Sales target cannot be negative.")]
        [Display(Name = "Sales Target")]
        public decimal SalesTarget { get; set; }

        public bool IsActive { get; set; } = true;
    }
}