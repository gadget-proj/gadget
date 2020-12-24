using System.Collections.Generic;

namespace Gadget.Messaging.Commands
{
    public interface IRegisterNewAgent
    {
        string Agent { get; }
        IEnumerable<Service> Services { get; }
    }
}