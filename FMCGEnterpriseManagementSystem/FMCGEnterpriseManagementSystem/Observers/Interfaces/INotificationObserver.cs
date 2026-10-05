// Title: Interfaces - C# Programming Guide
// Author: Microsoft
// Date: 18-03-2023
// Code version: C# .Net Core 10.0
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces

using FMCGEnterpriseManagementSystem.DTOs;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    // Defines the contract for classes that receive notification updates.
    public interface INotificationObserver
    {
        // Handles a notification received from the notification subject.
        Task HandleAsync(NotificationDto notificationDto);
    }
}