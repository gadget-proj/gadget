using System;

namespace Gadget.Messaging.Events
{
    public interface IHelloEvent
    {
        string Content { get; }
        DateTime CreatedAt { get; }
    }
}