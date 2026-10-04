// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Collections.Generic;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.DTOs;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    // Defines the contract for forecasting strategies.
    // Implementations of this interface are responsible for
    // generating forecast results from available business data.
    public interface IForecastingStrategy
    {
        // Asynchronously generates a collection of forecast results.
        //
        // Task is used because forecasting may involve database
        // queries or other operations that should not block the
        // application while the results are being generated.
        //
        // IEnumerable<ForecastResultDto> represents the collection
        // of forecast results returned by the strategy.
        Task<IEnumerable<ForecastResultDto>> GenerateForecastAsync();
    }
}