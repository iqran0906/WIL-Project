

/***************************************************************************************
*    Title: Interfaces - C# Programming Guide
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces
***************************************************************************************/
using FMCGEnterpriseManagementSystem.Observers.Interfaces;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    public interface INotificationSubject
    {
        void Attach(INotificationObserver observer);
        void Detach(INotificationObserver observer);
    }
}