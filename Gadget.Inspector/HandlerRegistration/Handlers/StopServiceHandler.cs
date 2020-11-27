using Gadget.Messaging.Commands;
using System.ServiceProcess;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class StopServiceHandler : IHandler<StopService>
    {
        public void StopService(StopService stop)
        {
            var serviceController = new ServiceController(stop.ServiceName);
            serviceController.Start();
        }
    }
}
