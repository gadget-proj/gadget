using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Gadget.Notifications.Domain.ValueObjects;

namespace Gadget.Notifications.Domain.Entities
{
    public class Notification2
    {
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        private readonly ICollection<Receiver> _receivers = new HashSet<Receiver>();
        public IEnumerable<Receiver> Receivers => _receivers.ToImmutableList();

        public void AddReceiver(Receiver receiver)
        {
            if (receiver is null)
            {
                return;
            }

            _receivers.Add(receiver);
        }
    }
}