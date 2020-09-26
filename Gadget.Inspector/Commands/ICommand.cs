using System.Collections.Generic;

namespace Gadget.Inspector.Commands
{
    public interface ICommand
    {
        public void Execute(ICollection<Service> services, string argument = "");
    }
}
