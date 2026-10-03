/***************************************************************************************
*    Title: Export Result Action Result
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Results/ExportResultActionResult.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Controller action return types in ASP.NET Core MVC
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core MVC
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
***************************************************************************************/
using FMCGEnterpriseManagementSystem.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Helpers
{
    public static class FileExportHelper
    {
        public static FileContentResult ToFileResult(ExportResultDto result)
        {
            return new FileContentResult(result.FileContents, result.ContentType)
            {
                FileDownloadName = result.FileName
            };
        }

        public static string BuildFileName(string baseName, string extension)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeName = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars()));
            return $"{safeName}_{timestamp}.{extension}";
        }
    }
}