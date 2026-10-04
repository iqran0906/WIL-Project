```csharp
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
    // Provides notification-related business logic for the application.
    // This service acts as a bridge between notification repositories,
    // notification subjects and notification observers.
    public class NotificationService : INotificationService
    {
        // Repository used to retrieve and manage stored notifications.
        private readonly INotificationRepository _notificationRepository;

        // Subject responsible for inventory-related notifications.
        private readonly InventoryNotificationSubject _inventorySubject;

        // Subject responsible for payment-related notifications.
        private readonly PaymentNotificationSubject _paymentSubject;

        // Observer responsible for sending email notifications.
        private readonly EmailNotificationObserver _emailObserver;

        // Observer responsible for creating system alerts.
        private readonly SystemAlertObserver _systemAlertObserver;

        // Constructor receives all required dependencies through
        // dependency injection.
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

            // Attach the system alert observer and email observer
            // to the inventory notification subject.
            _inventorySubject.Attach(_systemAlertObserver);
            _inventorySubject.Attach(_emailObserver);

            // Attach the same observers to the payment notification subject.
            _paymentSubject.Attach(_systemAlertObserver);
            _paymentSubject.Attach(_emailObserver);
        }

        // Sends a notification when a new quote is created.
        public async Task NotifyNewQuoteAsync(
            string quoteNumber,
            int quoteId,
            string customerName)
        {
            // Passes the quote information to the payment notification subject,
            // which then notifies its registered observers.
            await _paymentSubject.NotifyNewQuoteAsync(
                quoteNumber,
                quoteId,
                customerName);
        }

        // Retrieves all stored notifications.
        public async Task<List<NotificationViewModel>> GetAllAsync()
        {
            // Retrieves notification entities from the repository.
            var notifications = await _notificationRepository.GetAllAsync();

            // Converts each notification entity into a view model.
            return notifications.Select(MapToViewModel).ToList();
        }

        // Retrieves only unread notifications.
        public async Task<List<NotificationViewModel>> GetUnreadAsync()
        {
            // Retrieves unread notification records from the repository.
            var notifications = await _notificationRepository.GetUnreadAsync();

            // Converts the records into view models for the UI.
            return notifications.Select(MapToViewModel).ToList();
        }

        // Marks a specific notification as read.
        public async Task MarkAsReadAsync(int id)
        {
            // Delegates the update to the notification repository.
            await _notificationRepository.MarkAsReadAsync(id);
        }

        // Gets the number of unread notifications.
        public async Task<int> GetUnreadCountAsync()
        {
            // Retrieves the unread notification count from the repository.
            return await _notificationRepository.GetUnreadCountAsync();
        }

        // Sends a notification when product stock falls below a threshold.
        public async Task NotifyLowStockAsync(
            string productName,
            int currentQuantity,
            int threshold)
        {
            // Passes the stock information to the inventory notification subject.
            await _inventorySubject.NotifyLowStockAsync(
                productName,
                currentQuantity,
                threshold);
        }

        // Sends a notification when a new product/item is added.
        public async Task NotifyNewItemAddedAsync(
            string productName,
            int productId)
        {
            // Notifies observers through the inventory notification subject.
            await _inventorySubject.NotifyNewItemAddedAsync(
                productName,
                productId);
        }

        // Sends a notification when a new customer is created.
        public async Task NotifyNewCustomerAsync(
            string customerName,
            int customerId)
        {
            // Passes the customer information to the payment notification subject.
            await _paymentSubject.NotifyNewCustomerAsync(
                customerName,
                customerId);
        }

        // Sends a notification when a new invoice is created.
        public async Task NotifyNewInvoiceAsync(
            string invoiceNumber,
            int invoiceId,
            string customerName)
        {
            // Notifies the registered observers about the new invoice.
            await _paymentSubject.NotifyNewInvoiceAsync(
                invoiceNumber,
                invoiceId,
                customerName);
        }

        // Sends a notification when a quote has expired.
        public async Task NotifyQuoteExpiredAsync(
            string quoteNumber,
            int quoteId,
            string customerName)
        {
            // Passes the expired quote information to the notification subject.
            await _paymentSubject.NotifyQuoteExpiredAsync(
                quoteNumber,
                quoteId,
                customerName);
        }

        // Sends a notification when an invoice payment becomes overdue.
        public async Task NotifyOverduePaymentAsync(
            string invoiceNumber,
            int invoiceId,
            string customerName,
            decimal amountDue)
        {
            // Passes the overdue payment details to the payment notification subject.
            await _paymentSubject.NotifyOverduePaymentAsync(
                invoiceNumber,
                invoiceId,
                customerName,
                amountDue);
        }

        // Converts a Notification entity into a NotificationViewModel.
        // This keeps database entities separate from presentation models.
        private NotificationViewModel MapToViewModel(Notification notification)
        {
            return new NotificationViewModel
            {
                // Copies the notification's unique identifier.
                NotificationId = notification.NotificationId,

                // Converts the notification type into displayable text.
                TypeDisplay = notification.Type.ToString(),

                // Copies the notification title and message.
                Title = notification.Title,
                Message = notification.Message,

                // Indicates whether the notification has been read.
                IsRead = notification.IsRead,

                // Indicates whether an email notification was sent.
                EmailSent = notification.EmailSent,

                // Formats the notification creation date for display.
                CreatedDateDisplay =
                    notification.CreatedDate.ToString("dd MMM yyyy, HH:mm"),

                // Stores information about the entity associated
                // with the notification.
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType
            };
        }
    }
}
```
