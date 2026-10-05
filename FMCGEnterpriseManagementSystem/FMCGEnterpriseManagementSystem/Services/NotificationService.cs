// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly InventoryNotificationSubject _inventorySubject;
        private readonly PaymentNotificationSubject _paymentSubject;
        private readonly EmailNotificationObserver _emailObserver;
        private readonly SystemAlertObserver _systemAlertObserver;

        public NotificationService(
            INotificationRepository notificationRepository,
            InventoryNotificationSubject inventorySubject,
            PaymentNotificationSubject paymentSubject,
            EmailNotificationObserver emailObserver,
            SystemAlertObserver systemAlertObserver)
        {
            _notificationRepository = notificationRepository;
            _inventorySubject = inventorySubject;
            _paymentSubject = paymentSubject;
            _emailObserver = emailObserver;
            _systemAlertObserver = systemAlertObserver;

            // Wire up observers to subjects once, here, so callers don't need to worry about it
            _inventorySubject.Attach(_systemAlertObserver);
            _inventorySubject.Attach(_emailObserver);

            _paymentSubject.Attach(_systemAlertObserver);
            _paymentSubject.Attach(_emailObserver);
        }

        /*****************************
      *    Title: Observer design pattern
      *    Author: Microsoft
      *    Date: 02-06-2026
      *    Code version: .NET 10.0
      *    Availability: https://learn.microsoft.com/dotnet/standard/events/observer-design-pattern
      *****************************/
        public async Task NotifyNewQuoteAsync(string quoteNumber, int quoteId, string customerName)
        {
            await _paymentSubject.NotifyNewQuoteAsync(quoteNumber, quoteId, customerName);
        }


        /*****************************
       *    Title: Design the infrastructure persistence layer
       *    Author: Microsoft
       *    Date: 21-02-2023
       *    Code version: Not versioned (eBook guidance for .NET)
       *    Availability: https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design
       *****************************/

        // Retrieves all notifications through the repository and converts
        // each one into a view model for display.
        public async Task<List<NotificationViewModel>> GetAllAsync()
        {
            var notifications = await _notificationRepository.GetAllAsync();
            return notifications.Select(MapToViewModel).ToList();
        }

        // Retrieves only unread notifications and converts them into view models.
        public async Task<List<NotificationViewModel>> GetUnreadAsync()
        {
            var notifications = await _notificationRepository.GetUnreadAsync();
            return notifications.Select(MapToViewModel).ToList();
        }

        // Marks a single notification as read using its unique ID.
        public async Task MarkAsReadAsync(int id)
        {
            await _notificationRepository.MarkAsReadAsync(id);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _notificationRepository.GetUnreadCountAsync();
        }


        // Notifies the inventory subject that a product's stock has fallen
        // to or below its minimum threshold.
        public async Task NotifyLowStockAsync(string productName, int currentQuantity, int threshold)
        {
            await _inventorySubject.NotifyLowStockAsync(productName, currentQuantity, threshold);
        }

        // Notifies the inventory subject that a new product has been added.

        public async Task NotifyNewItemAddedAsync(string productName, int productId)
        {
            await _inventorySubject.NotifyNewItemAddedAsync(productName, productId);
        }

        // Notifies the inventory subject that a new customer has been added.

        public async Task NotifyNewCustomerAsync(string customerName, int customerId)
        {
            await _inventorySubject.NotifyNewCustomerAsync(customerName, customerId);
        }
        // Notifies the payment subject that a new invoice has been created.

        public async Task NotifyNewInvoiceAsync(string invoiceNumber, int invoiceId, string customerName)
        {
            await _paymentSubject.NotifyNewInvoiceAsync(invoiceNumber, invoiceId, customerName);
        }
        // Notifies the payment subject that a quotation has expired.

        public async Task NotifyQuoteExpiredAsync(string quoteNumber, int quoteId, string customerName)
        {
            await _paymentSubject.NotifyQuoteExpiredAsync(quoteNumber, quoteId, customerName);
        }
        // Notifies the payment subject that an invoice payment is overdue,
        // including the outstanding amount
        public async Task NotifyOverduePaymentAsync(string invoiceNumber, int invoiceId, string customerName, decimal amountDue)
        {
            await _paymentSubject.NotifyOverduePaymentAsync(invoiceNumber, invoiceId, customerName, amountDue);
        }

        // Converts a Notification model into a NotificationViewModel
        // with display-friendly values for the views.
        private NotificationViewModel MapToViewModel(Notification notification)
        {
            return new NotificationViewModel
            {
                NotificationId = notification.NotificationId,
                TypeDisplay = notification.Type.ToString(),
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                EmailSent = notification.EmailSent,
                // Formats the created date for display, e.g. 05 Oct 2026, 14:30.
                CreatedDateDisplay = notification.CreatedDate.ToString("dd MMM yyyy, HH:mm"),
                // Links the notification to the record it relates to (e.g. an invoice or quote).
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType
            };
        }
    }
}