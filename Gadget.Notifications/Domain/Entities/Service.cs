using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Gadget.Notifications.Domain.ValueObjects;

namespace Gadget.Notifications.Domain.Entities
{
    public sealed class Service
    {
        public Guid Id { get; }
        public string Agent { get; }
        public string Name { get; }
        private readonly ICollection<Webhook> _webhooks = new HashSet<Webhook>();

        public IEnumerable<Webhook> Webhooks => _webhooks.ToImmutableList();

        public Service(string agent, string name)
        {
            Agent = agent ?? throw new ArgumentNullException(nameof(agent));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Id = Guid.NewGuid();
        }

        public void AddWebhook(Webhook webhook) => _webhooks.Add(webhook);
    }
}