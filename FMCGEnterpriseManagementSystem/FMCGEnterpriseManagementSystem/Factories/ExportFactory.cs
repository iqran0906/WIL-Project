/***************************************************************************************
*    Title: Export Factory
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Factories/ExportFactory.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Dependency injection in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Strategies;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;

namespace FMCGEnterpriseManagementSystem.Factories
{
    // Selects the appropriate export strategy based on the requested export type.
    public class ExportFactory
    {
        private readonly IEnumerable<IExportStrategy> _strategies;

        // Receives the available export strategies through dependency injection.
        public ExportFactory(IEnumerable<IExportStrategy> strategies)
        {
            _strategies = strategies;
        }

        // Returns the strategy that handles the requested PDF or Excel export.
        public IExportStrategy GetStrategy(ExportType type)
        {
            return type switch
            {
                ExportType.Pdf => _strategies.OfType<PdfExportStrategy>().First(),
                ExportType.Excel => _strategies.OfType<ExcelExportStrategy>().First(),
                _ => throw new ArgumentException($"Unsupported export type: {type}")
            };
        }
    }
}