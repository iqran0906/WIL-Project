// Title: Enums in C#
// Author: Code Maze
// Date: 05-10-2018
// Code version: C# / .NET
// Availability: https://code-maze.com/csharp-enum/
//
namespace FMCGEnterpriseManagementSystem.Models
{
    // Defines the different types of notifications supported by the system.
    public enum NotificationType
    {
        // Notification generated when stock reaches a low level.
        LowStock,

        // Notification generated when a new quote is created.
        NewQuote,

        // Notification generated when a quote reaches its expiry date.
        QuoteExpired,

        // Notification generated when a new invoice is created.
        NewInvoice,

        // Notification generated when a new customer is added.
        NewCustomer,

        // Notification generated when a new item is added.
        NewItem,

        // Notification generated when a customer payment becomes overdue.
        OverduePayment,

        // Notification used for general system alerts.
        SystemAlert
    }
}