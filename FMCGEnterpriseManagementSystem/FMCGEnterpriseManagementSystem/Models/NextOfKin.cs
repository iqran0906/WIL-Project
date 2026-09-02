using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class NextOfKin
    {
        [Key]
        [StringLength(20)]
        public string NextOfKinID { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string EmployeeID { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}