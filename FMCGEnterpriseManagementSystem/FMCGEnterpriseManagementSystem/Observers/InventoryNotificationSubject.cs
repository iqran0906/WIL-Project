/***************************************************************************************
*    Title: Stock Notification Observer
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Observers/StockNotificationObserver.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Dependency injection in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
***************************************************************************************/

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Observers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers
{
    public class InventoryNotificationSubject : INotificationSubject
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

        public async Task NotifyLowStockAsync(string productName, int currentQuantity, int threshold)
        {
            var dto = new NotificationDto
            {
                Type = NotificationType.LowStock,
                Title = "Inventory is low",
                Message = $"{productName} is running low. Current stock: {currentQuantity} (threshold: {threshold}).",
                RelatedEntityType = "Product"
            };

            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

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

            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }

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

            foreach (var observer in _observers)
            {
                await observer.HandleAsync(dto);
            }
        }
    }
}