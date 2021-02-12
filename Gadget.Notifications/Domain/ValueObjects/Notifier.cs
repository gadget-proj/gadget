using System;
using Gadget.Notifications.Domain.Enums;

namespace Gadget.Notifications.Domain.ValueObjects
{
    public class Notifier : INotifier
    {
        public DateTime CreatedAt { get; }
        public NotifierType NotifierType { get; }
        public string AgentName { get; }
        public string ServiceName { get; }
        public string Receiver { get; }

        private Notifier()
        {
            
        }
        public Notifier(string agentName, string serviceName, string receiver, NotifierType notifierType)
        {
            AgentName = agentName ?? throw new ArgumentNullException(nameof(agentName));
            ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            NotifierType = notifierType;

            CreatedAt = DateTime.UtcNow;
        }
    }

    public interface INotifier
    {
        string AgentName { get; }
        string ServiceName { get; }
        string Receiver { get; }
    }
}