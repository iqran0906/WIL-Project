using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    public class PaymentNotificationSubject : INotificationSubject
    {
        private readonly List<INotificationObserver> _observers = new List<INotificationObserver>();

        public void Attach(INotificationObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(INotificationObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyNewInvoiceAsync(string invoiceNumber, int invoiceId, string customerName)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.NewInvoice,
                Title = "New invoice created",
                Message = $"Invoice {invoiceNumber} was created for {customerName}.",
                RelatedEntityId = invoiceId,
                RelatedEntityType = "Invoice"
            };

            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        public async Task NotifyQuoteExpiredAsync(string quoteNumber, int quoteId, string customerName)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.QuoteExpired,
                Title = "Quote expired",
                Message = $"Quote {quoteNumber} for {customerName} has expired.",
                RelatedEntityId = quoteId,
                RelatedEntityType = "Quote"
            };

            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        public async Task NotifyOverduePaymentAsync(string invoiceNumber, int invoiceId, string customerName, decimal amountDue)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.OverduePayment,
                Title = "Payment overdue",
                Message = $"Payment for invoice {invoiceNumber} ({customerName}) is overdue. Amount due: R{amountDue:N2}.",
                RelatedEntityId = invoiceId,
                RelatedEntityType = "Invoice"
            };

            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }
    }
}