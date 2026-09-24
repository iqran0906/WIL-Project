using FMCGEnterpriseManagementSystem.DTOs;

namespace FMCGEnterpriseManagementSystem.Strategies.Interfaces
{
    public interface IExportStrategy
    {
        ExportResultDto Export<T>(IEnumerable<T> data, string title);
    }
}
