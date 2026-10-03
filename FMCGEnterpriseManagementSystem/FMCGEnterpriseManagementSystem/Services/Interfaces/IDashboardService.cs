/***************************************************************************************
*    Title: Dashboard Service Interface
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IDashboardService.cs
***************************************************************************************/

using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAnalyticsAsync(int months = 12);
    }
}