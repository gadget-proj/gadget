using System;
using MassTransit;

namespace Gadget.Messaging.Contracts.Commands.v1
{
    public interface IStopService : CorrelatedBy<Guid>
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}