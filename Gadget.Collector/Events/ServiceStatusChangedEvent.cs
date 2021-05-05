using System;

namespace Gadget.Collector.Events
{
    public record ServiceStatusChangedEvent(Guid Id, string Agent, string Name, string Status, DateTime Date);
}