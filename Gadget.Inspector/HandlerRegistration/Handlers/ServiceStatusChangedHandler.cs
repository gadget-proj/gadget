using Gadget.Messaging.Events;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class ServiceStatusChangedHandler : IHandler<ServiceStatusChanged>
    {
        public void ServiceStatusChanged()
        {
            System.Console.WriteLine("Weszło status changed");
        }
    }
}
