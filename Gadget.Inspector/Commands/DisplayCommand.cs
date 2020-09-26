using System.Collections.Generic;

namespace Gadget.Inspector.Commands
{
    class DisplayCommand : ICommand
    {
        public void Execute(ICollection<Service> services, string argument = "")
        {
            foreach (var service in services)
            {
                ServiceLogger.Log(service.ToString(),service.ServiceController.Status);
            }
        }
    }
}
