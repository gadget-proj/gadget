using System.Collections.Generic;
using System.Linq;

namespace Gadget.Inspector
{
    class RestartCommand : ICommand
    {
        public void Execute(ICollection<Service> services, string argument = "")
        {
            var service = services.SingleOrDefault(s => s.Name.ToLower() == argument.ToLower());
            if (service is null)
            {
                return;
            }
            service.Restart();
        }
    }
}
