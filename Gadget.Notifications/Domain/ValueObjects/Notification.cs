using System;

namespace Gadget.Notifications.Domain.ValueObjects
{
    public class Notification
    {
        public string Agent { get; }
        public string ServiceName { get; }
        public string Status { get; }
        public Uri Webhook { get; }

        public Notification(string agent, string serviceName, string status, Uri webhook)
        {
            Agent = agent ?? throw new ArgumentNullException(nameof(agent));
            ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            Status = status ?? throw new ArgumentNullException(nameof(status));
            Webhook = webhook;
        }
    }
}