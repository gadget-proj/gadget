using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gadget.Notifications.Domain.Entities
{
    public class Service
    {
        public string Name { get; }
        private readonly ICollection<Notification2> _notifications = new HashSet<Notification2>();
        public IEnumerable<Notification2> Notifications => _notifications.ToImmutableList();

        public Service(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void AddNotification(Notification2 notification)
        {
            if (notification is null)
            {
                return;
            }

            _notifications.Add(notification);
        }
    }
}