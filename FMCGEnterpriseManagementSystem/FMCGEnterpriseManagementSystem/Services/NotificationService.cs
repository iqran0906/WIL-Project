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

        public async Task<List<NotificationViewModel>> GetAllAsync()
        {
            var notifications = await _notificationRepository.GetAllAsync();
            return notifications.Select(MapToViewModel).ToList();
        }

        public async Task<List<NotificationViewModel>> GetUnreadAsync()
        {
            var notifications = await _notificationRepository.GetUnreadAsync();
            return notifications.Select(MapToViewModel).ToList();
        }

        public async Task MarkAsReadAsync(int id)
        {
            await _notificationRepository.MarkAsReadAsync(id);
        }

        public async Task NotifyLowStockAsync(string productName, int currentQuantity, int threshold)
        {
            await _inventorySubject.NotifyLowStockAsync(productName, currentQuantity, threshold);
        }

        public async Task NotifyNewItemAddedAsync(string productName, int productId)
        {
            await _inventorySubject.NotifyNewItemAddedAsync(productName, productId);
        }

        public async Task NotifyNewCustomerAsync(string customerName, int customerId)
        {
            await _inventorySubject.NotifyNewCustomerAsync(customerName, customerId);
        }

        public async Task NotifyNewInvoiceAsync(string invoiceNumber, int invoiceId, string customerName)
        {
            await _paymentSubject.NotifyNewInvoiceAsync(invoiceNumber, invoiceId, customerName);
        }

        public async Task NotifyQuoteExpiredAsync(string quoteNumber, int quoteId, string customerName)
        {
            await _paymentSubject.NotifyQuoteExpiredAsync(quoteNumber, quoteId, customerName);
        }

        public async Task NotifyOverduePaymentAsync(string invoiceNumber, int invoiceId, string customerName, decimal amountDue)
        {
            await _paymentSubject.NotifyOverduePaymentAsync(invoiceNumber, invoiceId, customerName, amountDue);
        }

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
                CreatedDateDisplay = notification.CreatedDate.ToString("dd MMM yyyy, HH:mm"),
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType
            };
        }
    }
}