using System;

namespace Gadget.Messaging.Contracts.Events.v1
{
    public interface IServiceStatusChanged
    {
        string Agent { get; }
        string Name { get; }
        string Status { get; }
        DateTime Date { get; }
    }
}