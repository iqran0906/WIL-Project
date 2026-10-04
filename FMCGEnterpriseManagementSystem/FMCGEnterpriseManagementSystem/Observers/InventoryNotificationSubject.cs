// Title: Dependency injection in ASP.NET Core
// Author: Microsoft
// Date: 18-09-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    // Subject responsible for notifying observers about inventory-related events.
    public class InventoryNotificationSubject : INotificationSubject
    {
        // Stores the observers that are currently subscribed to inventory notifications.
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

        // Creates and sends a low-stock notification to all subscribed observers.
        public async Task NotifyLowStockAsync(
            string productName,
            int currentQuantity,
            int threshold)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.LowStock,
                Title = "Inventory is low",
                Message = $"{productName} is running low. Current stock: {currentQuantity} (threshold: {threshold}).",
                RelatedEntityType = "Product"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        // Creates and sends a notification when a new inventory item is added.
        public async Task NotifyNewItemAddedAsync(string productName, int productId)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.NewItem,
                Title = "New item added",
                Message = $"A new item, {productName}, has been added to inventory.",
                RelatedEntityId = productId,
                RelatedEntityType = "Product"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

        // Creates and sends a notification when a new customer is added.
        public async Task NotifyNewCustomerAsync(string customerName, int customerId)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.NewCustomer,
                Title = "New customer added",
                Message = $"A new customer, {customerName}, has been added.",
                RelatedEntityId = customerId,
                RelatedEntityType = "Customer"
            };

            // Notifies each registered observer.
            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }
    }
}