using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gadget.Notifications.Domain.Entities
{
    public sealed class Service
    {
        public string Agent { get; }
        public string Name { get; }
        private readonly ICollection<Uri> _webhooks = new HashSet<Uri>();

        public ICollection<Uri> Webhooks => _webhooks.ToImmutableList();

        public Service(string agent, string name)
        {
            Agent = agent ?? throw new ArgumentNullException(nameof(agent));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void AddWebhook(Uri webhook) => _webhooks.Add(webhook);
    }
}