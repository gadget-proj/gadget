using System;
using MassTransit;

namespace Gadget.Messaging.Contracts.Responses
{
    public interface IActionResultResponse : CorrelatedBy<Guid>
    {
        string Reason { get; }
        bool Success { get; }
    }
}