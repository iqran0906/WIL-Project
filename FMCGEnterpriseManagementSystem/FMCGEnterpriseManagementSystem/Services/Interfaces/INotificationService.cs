using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationViewModel>> GetAllAsync();
        Task<List<NotificationViewModel>> GetUnreadAsync();
        Task MarkAsReadAsync(int id);

        Task NotifyLowStockAsync(string productName, int currentQuantity, int threshold);
        Task NotifyNewItemAddedAsync(string productName, int productId);
        Task NotifyNewCustomerAsync(string customerName, int customerId);
        Task NotifyNewInvoiceAsync(string invoiceNumber, int invoiceId, string customerName);
        Task NotifyQuoteExpiredAsync(string quoteNumber, int quoteId, string customerName);
        Task NotifyOverduePaymentAsync(string invoiceNumber, int invoiceId, string customerName, decimal amountDue);
    }
}