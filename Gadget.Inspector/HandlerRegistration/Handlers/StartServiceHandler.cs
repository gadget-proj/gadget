using Gadget.Messaging.Commands;
using System.ServiceProcess;

namespace Gadget.Inspector.HandlerRegistration.Handlers
{
    public class StartServiceHandler : IHandler<StartService>
    {
        public void StartService(StartService start)
        {
            var serviceController = new ServiceController(start.ServiceName);
            serviceController.Start();
        }
    }
}
