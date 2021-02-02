using System;

namespace Gadget.Notifications.Domain.ValueObjects
{
    public sealed class Webhook
    {
        public Uri Uri { get; }
        public DateTime CreatedAt { get; }

        public Webhook(Uri uri)
        {
            Uri = uri;
            CreatedAt = DateTime.UtcNow;
        }
        private Webhook(){}
    }
}