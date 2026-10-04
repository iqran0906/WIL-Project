

/***************************************************************************************
*    Title: Role-based authorization in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/aspnet/core/security/authorization/roles
***************************************************************************************/


namespace FMCGEnterpriseManagementSystem.Configurations
{
    // Stores the application's role names in one central location.
    // This helps keep role names consistent when used for access control.
    public static class RoleSettings
    {
        public const string Administrator = "Administrator";
        public const string Employee = "Employee";
        public const string SalesRepresentative = "SalesRepresentative";
    }
}