using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace Gadget.Inspector.Commands
{
    class CreateCommand : ICommand
    {
        public void Execute(ICollection<Service> services, string argument)
        {
            if (argument.ToLower() == "all")
            {
                foreach (var serviceController in ServiceController.GetServices())
                {
                    RegisterNewService(serviceController.ServiceName, services);
                }
                return;
            }
            RegisterNewService(argument, services);
        }

        private static void RegisterNewService(string name, ICollection<Service> services)
        {
            var s = new Service(name);
            s.AddStatusHandler(ServiceControllerStatus.Stopped,
                controller => { Console.WriteLine($"{Environment.NewLine}> {controller.DisplayName} is stopped"); });
            s.AddStatusHandler(ServiceControllerStatus.Running,
                controller =>
                    Console.WriteLine($"{Environment.NewLine}> {controller.DisplayName} is well and running ♥"));
            services.Add(s);
        }
    }
}
