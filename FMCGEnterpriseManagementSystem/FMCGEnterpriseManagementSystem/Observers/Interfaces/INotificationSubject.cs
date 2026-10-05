// Title: Interfaces - C# Programming Guide
// Author: Microsoft
// Date: 18-03-2023
// Code version: C# .Net Core 10.0
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces

using FMCGEnterpriseManagementSystem.Observers.Interfaces;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    // Defines the contract for a notification subject that manages observers.
    public interface INotificationSubject
    {
        // Adds an observer so that it can receive notification updates.
        void Attach(INotificationObserver observer);

        // Removes an observer so that it no longer receives notification updates.
        void Detach(INotificationObserver observer);
    }
}