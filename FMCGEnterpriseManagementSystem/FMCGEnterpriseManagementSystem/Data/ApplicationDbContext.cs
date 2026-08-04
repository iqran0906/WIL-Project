using FMCGEnterpriseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.CostExVat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.CostIncVat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Supplier>()
                .Property(s => s.CreditLimit)
                .HasPrecision(18, 2);
        }
    }
}