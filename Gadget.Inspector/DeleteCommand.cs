using System;
using System.Collections.Generic;
using System.Linq;

namespace Gadget.Inspector
{
    class DeleteCommand : ICommand

    {
        public void Execute(ICollection<Service> services, string argument)
        {
            var s = services.SingleOrDefault(svc => svc.ServiceController.ServiceName.ToLower() == argument.ToLower());
            services.Remove(s);
        }
    }
}
