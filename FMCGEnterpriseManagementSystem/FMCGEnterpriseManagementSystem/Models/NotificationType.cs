// Purpose: Types of notification the system can raise (low stock, new invoice, ...).
// Authors: Sayali-St10458649, Naseeha27 (from git history)

namespace FMCGEnterpriseManagementSystem.Models
{
    public enum NotificationType
    {
        LowStock,
        QuoteExpired,
        NewInvoice,
        NewCustomer,
        NewItem,
        OverduePayment,
        SystemAlert
    }
}