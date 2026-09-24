using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public List<Invoice> RecentInvoices { get; set; } = new();
    }
}