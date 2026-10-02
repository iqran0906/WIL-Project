// Purpose: Factory pattern: returns the PDF or Excel export strategy for a requested format.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Strategies;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;

namespace FMCGEnterpriseManagementSystem.Factories
{
    public class ExportFactory
    {
        private readonly IEnumerable<IExportStrategy> _strategies;

        public ExportFactory(IEnumerable<IExportStrategy> strategies)
        {
            _strategies = strategies;
        }

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