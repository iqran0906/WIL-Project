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