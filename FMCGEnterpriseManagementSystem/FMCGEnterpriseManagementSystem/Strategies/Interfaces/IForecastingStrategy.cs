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