// Purpose: Observer pattern: contract for a subject that notifies its observers.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Observers.Interfaces;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    public interface INotificationSubject
    {
        void Attach(INotificationObserver observer);
        void Detach(INotificationObserver observer);
    }
}