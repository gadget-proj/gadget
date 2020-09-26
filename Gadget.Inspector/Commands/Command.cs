using System;
using System.Collections.Generic;
using System.Linq;
using Gadget.Inspector.Commands;

namespace Gadget.Inspector
{

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
                "restart" => CommandAction.Restart,
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
            var command = CommandFactory.Create(Action);
            command.Execute(services, Target);
        }
    }
}