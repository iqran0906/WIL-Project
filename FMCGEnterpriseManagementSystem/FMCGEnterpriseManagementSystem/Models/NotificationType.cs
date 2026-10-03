/***************************************************************************************
*    Title: Notification Type
*    Author: Sayali-St10458649, Naseeha27
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Models/NotificationType.cs
***************************************************************************************/
namespace FMCGEnterpriseManagementSystem.Models
{
    public enum NotificationType
    {
        LowStock,
        NewQuote,
        QuoteExpired,
        NewInvoice,
        NewCustomer,
        NewItem,
        OverduePayment,
        SystemAlert
    }
}