namespace FMCGEnterpriseManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        // Foreign Key changed to int
        public int SupplierId { get; set; }

        // Navigation Property
        public Supplier? Supplier { get; set; }

        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal CostExVat { get; set; }
        public decimal CostIncVat { get; set; }
        public decimal SellingPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}