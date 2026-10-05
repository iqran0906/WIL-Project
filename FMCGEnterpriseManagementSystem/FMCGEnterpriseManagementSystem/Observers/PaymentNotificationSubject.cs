// Title: Dependency injection in ASP.NET Core
// Author: Microsoft
// Date: 18-09-2024
// Code version: ASP.NET Core 10,0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    // Subject responsible for notifying observers about quote and payment events.
    public class PaymentNotificationSubject : INotificationSubject
    {
        // Stores the observers subscribed to payment-related notifications.
        private readonly List<INotificationObserver> _observers =
            new List<INotificationObserver>();

        // Adds an observer to the notification list.
        public void Attach(INotificationObserver observer)
        {
            _observers.Add(observer);
        }

        // Removes an observer from the notification list.
        public void Detach(INotificationObserver observer)
        {
            _observers.Remove(observer);
        }

        // Creates and sends a notification when a new quote is created.
        public async Task NotifyNewQuoteAsync(
            string quoteNumber,
            int quoteId,
            string customerName)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.NewQuote,
                Title = "New quote created",
                Message = $"Quote {quoteNumber} was created for {customerName}.",
                RelatedEntityId = quoteId,
                RelatedEntityType = "Quote"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        // Creates and sends a notification when a new invoice is created.
        public async Task NotifyNewInvoiceAsync(
            string invoiceNumber,
            int invoiceId,
            string customerName)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.NewInvoice,
                Title = "New invoice created",
                Message = $"Invoice {invoiceNumber} was created for {customerName}.",
                RelatedEntityId = invoiceId,
                RelatedEntityType = "Invoice"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        // Creates and sends a notification when a quote expires.
        public async Task NotifyQuoteExpiredAsync(
            string quoteNumber,
            int quoteId,
            string customerName)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.QuoteExpired,
                Title = "Quote expired",
                Message = $"Quote {quoteNumber} for {customerName} has expired.",
                RelatedEntityId = quoteId,
                RelatedEntityType = "Quote"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        // Creates and sends a notification when an invoice payment becomes overdue.
        public async Task NotifyOverduePaymentAsync(
            string invoiceNumber,
            int invoiceId,
            string customerName,
            decimal amountDue)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.OverduePayment,
                Title = "Payment overdue",
                Message = $"Payment for invoice {invoiceNumber} ({customerName}) is overdue. Amount due: R{amountDue:N2}.",
                RelatedEntityId = invoiceId,
                RelatedEntityType = "Invoice"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }
    }
}