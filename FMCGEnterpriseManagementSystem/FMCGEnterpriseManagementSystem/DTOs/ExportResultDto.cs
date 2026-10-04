

/***************************************************************************************
*    Title: Classes and Objects - C# Programming Guide
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
***************************************************************************************/
namespace FMCGEnterpriseManagementSystem.DTOs
{
    // Stores the information required to return an exported file to the user.
    public class ExportResultDto
    {
        // Contains the generated file data.
        public byte[] FileContents { get; set; }

        // Contains the name that will be used for the exported file.
        public string FileName { get; set; }

        // Contains the MIME type used to identify the exported file format.
        public string ContentType { get; set; }
    }
}