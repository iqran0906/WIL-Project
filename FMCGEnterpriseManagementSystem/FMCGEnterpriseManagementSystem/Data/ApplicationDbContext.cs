using FMCGEnterpriseManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteItem> QuoteItems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<NextOfKin> NextOfKins { get; set; }
        public DbSet<SalesRepresentative> SalesRepresentatives { get; set; }
        public DbSet<StockBatch> StockBatches { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.CostExVat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.CostIncVat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasPrecision(18, 2);

            // Supplier decimal precision
            modelBuilder.Entity<Supplier>()
                .Property(s => s.CreditLimit)
                .HasPrecision(18, 2);

            // Employee -> NextOfKin
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.NextOfKin)
                .WithOne(n => n.Employee)
                .HasForeignKey<NextOfKin>(n => n.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee -> Identity User
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee -> SalesRepresentative
            modelBuilder.Entity<SalesRepresentative>()
                .HasOne(sr => sr.Employee)
                .WithOne()
                .HasForeignKey<SalesRepresentative>(sr => sr.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            // SalesRepresentative -> Customers
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.SalesRepresentative)
                .WithMany(sr => sr.Customers)
                .HasForeignKey(c => c.SalesRepresentativeId)
                .OnDelete(DeleteBehavior.SetNull);

            // SalesRepresentative -> Quotes
            modelBuilder.Entity<Quote>()
                .HasOne(q => q.SalesRepresentative)
                .WithMany(sr => sr.Quotes)
                .HasForeignKey(q => q.SalesRepresentativeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Supplier -> Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany()
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product -> Inventory
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithOne()
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Inventory -> StockBatches
            modelBuilder.Entity<StockBatch>()
                .HasOne(sb => sb.Inventory)
                .WithMany(i => i.StockBatches)
                .HasForeignKey(sb => sb.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // SalesRepresentative decimal precision
            modelBuilder.Entity<SalesRepresentative>()
                .Property(sr => sr.Salary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesRepresentative>()
                .Property(sr => sr.CommissionRate)
                .HasPrecision(5, 2);

            modelBuilder.Entity<SalesRepresentative>()
                .Property(sr => sr.SalesTarget)
                .HasPrecision(18, 2);
        }
    }
}