/***************************************************************************************
*    Title: Forecasting Repository Interface
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IForecastingRepository.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IForecastingRepository
    {
        // Retrieves items that need restocking along with cost data
        Task<IEnumerable<Inventory>> GetLowStockForRestockForecastAsync();

        // Retrieves inventory with category data for turnover and demand projection
        Task<IEnumerable<Inventory>> GetInventoryForDemandAnalysisAsync();
    }
}