using System;
using MassTransit;

namespace Gadget.Messaging.Contracts.Commands.v1
{
    public interface IStartService : CorrelatedBy<Guid>
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}