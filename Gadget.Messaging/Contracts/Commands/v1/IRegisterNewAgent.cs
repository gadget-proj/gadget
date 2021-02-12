using System.Collections.Generic;

namespace Gadget.Messaging.Contracts.Commands.v1
{
    public interface IRegisterNewAgent
    {
        string Agent { get; }
        string Address { get; }
        IEnumerable<object> Services { get; }
    }
}