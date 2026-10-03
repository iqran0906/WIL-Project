/***************************************************************************************
*    Title: Forecast Strategy
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Strategies/Interfaces/IForecastingStrategy.cs
***************************************************************************************/
using System.Collections.Generic;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.DTOs;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    public interface IForecastingStrategy
    {
        Task<IEnumerable<ForecastResultDto>> GenerateForecastAsync();
    }
}