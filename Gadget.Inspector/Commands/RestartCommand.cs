using System.Collections.Generic;
using System.Linq;

namespace Gadget.Inspector.Commands
{
    class RestartCommand : ICommand
    {
        public void Execute(ICollection<Service> services, string argument = "")
        {
            var service = services.SingleOrDefault(s => s.Name.ToLower() == argument.ToLower());
            service?.Restart();
        }
    }
}
