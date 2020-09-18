using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Gadget.Inspector
{
    public enum CommandAction
    {
        Create,
        Delete,
        Display
    }

    public class Command
    {
        public CommandAction Action { get; private set; }
        public string Target { get; private set; }

        public Command(string command)
        {
            var tokens = command.Trim().ToLower().Split(" ");
            var action = tokens.FirstOrDefault();
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentException();
            }

            Action = action switch
            {
                "create" => CommandAction.Create,
                "delete" => CommandAction.Delete,
                "display" => CommandAction.Display,
                _ => throw new ApplicationException("Invalid command provided")
            };
            var target = tokens.ElementAtOrDefault(1);
            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentException();
            }

            Target = target;
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

        public void Execute(ICollection<Service> services)
        {
            switch (Action)
            {
                case CommandAction.Create:
                    if (Target.ToLower() == "all")
                    {
                        foreach (var serviceController in ServiceController.GetServices())
                        {
                            RegisterNewService(serviceController.ServiceName, services);
                        }
                        break;
                    }

                    RegisterNewService(Target, services);
                    break;
                case CommandAction.Delete:
                    var s = services.SingleOrDefault(svc => svc.ServiceController.ServiceName.ToLower() == Target);
                    services.Remove(s);
                    break;
                case CommandAction.Display:
                    foreach (var service in services)
                    {
                        ServiceLogger.Log($"[SERVICE] : {service.ServiceController.ServiceName} " +
                                          $"[STATUS] : {service.ServiceController.Status}",
                            service.ServiceController.Status);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}