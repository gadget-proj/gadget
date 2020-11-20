using Gadget.Messaging.Events;
using System;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class ServiceStatusChangedHandler : IHandler<ServiceStatusChanged>
    {
        public void ServiceStatusChanged(ServiceStatusChanged status)
        {
            Console.WriteLine("Lucek lucek lucek");
        }
    }
}
