using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Execute(ICollection<Service> services)
        {
            switch (Action)
            {
                case CommandAction.Create:
                    services.Add(new Service(Target));
                    break;
                case CommandAction.Delete:
                    var s = services.SingleOrDefault(svc => svc.Name.ToLower() == Target);
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