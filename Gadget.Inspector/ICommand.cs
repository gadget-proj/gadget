using System.Collections.Generic;

namespace Gadget.Inspector
{
    public interface ICommand
    {
        public void Execute(ICollection<Service> services, string argument = "");
    }
}
