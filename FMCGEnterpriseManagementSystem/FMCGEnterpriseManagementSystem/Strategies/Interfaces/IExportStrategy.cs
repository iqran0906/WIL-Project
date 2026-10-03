/***************************************************************************************
*    Title: Excel Report Export Strategy
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Strategies/Interfces/IExportStrategy.cs
***************************************************************************************/
using FMCGEnterpriseManagementSystem.DTOs;

namespace FMCGEnterpriseManagementSystem.Strategies.Interfaces
{
    public interface IExportStrategy
    {
        ExportResultDto Export<T>(IEnumerable<T> data, string title);
    }
}
