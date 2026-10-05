// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to retrieve, manage and create
    // notifications within the system.
    public interface INotificationService
    {
        // Retrieves all notifications.
        Task<List<NotificationViewModel>> GetAllAsync();

        // Retrieves only notifications that have not been read.
        Task<List<NotificationViewModel>> GetUnreadAsync();

        // Marks a specific notification as read using its ID.
        Task MarkAsReadAsync(int id);

        // Marks all unread notifications as read.
        Task MarkAllAsReadAsync();

        // Retrieves the total number of unread notifications.
        Task<int> GetUnreadCountAsync();

        // Creates a notification when a product's stock reaches
        // or falls below the specified threshold.
        Task NotifyLowStockAsync(string productName, int currentQuantity, int threshold);

        // Creates a notification when a new item is added to the system.
        Task NotifyNewItemAddedAsync(string productName, int productId);

        // Creates a notification when a new customer is added.
        Task NotifyNewCustomerAsync(string customerName, int customerId);

        // Creates a notification when a new invoice is created.
        Task NotifyNewInvoiceAsync(string invoiceNumber, int invoiceId, string customerName);

        // Creates a notification when a quote has expired.
        Task NotifyQuoteExpiredAsync(string quoteNumber, int quoteId, string customerName);

        // Creates a notification when an invoice has an overdue payment.
        Task NotifyOverduePaymentAsync(string invoiceNumber, int invoiceId, string customerName, decimal amountDue);

        // Creates a notification when a new quote is created.
        Task NotifyNewQuoteAsync(string quoteNumber, int quoteId, string customerName);
    }
}