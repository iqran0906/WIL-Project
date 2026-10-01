// Purpose: Strategy pattern: contract for exporting a list of rows to a file.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.DTOs;

namespace FMCGEnterpriseManagementSystem.Strategies.Interfaces
{
    public interface IExportStrategy
    {
        ExportResultDto Export<T>(IEnumerable<T> data, string title);
    }
}
