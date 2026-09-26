using FMCGEnterpriseManagementSystem.Observers.Interfaces;

namespace FMCGEnterpriseManagementSystem.Observers.Interfaces
{
    public interface INotificationSubject
    {
        void Attach(INotificationObserver observer);
        void Detach(INotificationObserver observer);
    }
}