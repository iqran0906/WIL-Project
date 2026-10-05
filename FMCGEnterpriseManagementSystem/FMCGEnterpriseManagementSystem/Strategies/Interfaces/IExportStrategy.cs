// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.DTOs;

namespace FMCGEnterpriseManagementSystem.Strategies.Interfaces
{
    // Defines the contract for export strategies.
    // Different export implementations can use this interface to
    // convert collections of data into a standard ExportResultDto.
    public interface IExportStrategy
    {
        // Generic export method that accepts a collection of data
        // and a title for the generated export.
        //
        // The generic type T allows the strategy to work with
        // different types of data without changing the interface.
        //
        // IEnumerable<T> represents the collection of records
        // that need to be exported.
        //
        // The title parameter allows the generated export to
        // identify or display the relevant report title.
        //
        // ExportResultDto provides a standard result structure
        // that can contain the generated export information.
        ExportResultDto Export<T>(IEnumerable<T> data, string title);
    }
}
