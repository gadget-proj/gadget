using System;

namespace Gadget.Messaging.Contracts.Events.v1
{
    public interface IActionFailed
    {
        Guid CorrelationId { get; }
        string Agent { get; }
        string Service { get; }
        string Action { get; }
        string Reason { get; }
        DateTime Date { get; }
    }
}