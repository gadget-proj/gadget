using System;
using Gadget.Notifications.Domain.Enums;

namespace Gadget.Notifications.Domain.ValueObjects
{
    public class Notifier : INotifier
    {
        public DateTime CreatedAt { get; }
        public NotifierType NotifierType { get; }
        public string Receiver { get; }

        private Notifier()
        {
            
        }
        public Notifier(string agentName, string serviceName, string receiver, NotifierType notifierType)
        {
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            NotifierType = notifierType;

            CreatedAt = DateTime.UtcNow;
        }
    }

    public interface INotifier
    {
        string Receiver { get; }
    }
}