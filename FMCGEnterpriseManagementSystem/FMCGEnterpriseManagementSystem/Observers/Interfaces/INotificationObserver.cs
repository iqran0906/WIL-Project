/***************************************************************************************
*    Title: Notification Observer Interface
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Observers/Interfaces/INotificationObserver.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Interfaces - C# Programming Guide
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces
***************************************************************************************/
using FMCGEnterpriseManagementSystem.DTOs;
using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    public interface INotificationObserver
    {
        Task HandleAsync(NotificationDto notificationDto);
    }
}