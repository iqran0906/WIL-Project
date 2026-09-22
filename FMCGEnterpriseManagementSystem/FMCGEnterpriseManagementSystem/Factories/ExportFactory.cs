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