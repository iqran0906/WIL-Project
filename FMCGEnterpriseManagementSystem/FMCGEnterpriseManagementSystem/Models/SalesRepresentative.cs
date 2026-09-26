using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class SalesRepresentative
    {
        [Key]
        public int SalesRepresentativeId { get; set; }
      
        [Required]
        [StringLength(20)]
        public string EmployeeID { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string SalesRepCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Area { get; set; }

        public decimal Salary { get; set; }

        public decimal CommissionRate { get; set; }

        public decimal SalesTarget { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Employee Employee { get; set; } = null!;

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();

        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    }
}