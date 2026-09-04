namespace FMCGEnterpriseManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public decimal CostExVat { get; set; }

        public decimal CostIncVat { get; set; }

        public decimal SellingPrice { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}