using System;
using System.Collections.Generic;
using MassTransit;

namespace Gadget.Messaging.Contracts.Commands.v1
{
    public interface IRegisterNewAgent : CorrelatedBy<Guid>
    {
        string Agent { get; }
        string Address { get; }
        IEnumerable<object> Services { get; }
    }
}