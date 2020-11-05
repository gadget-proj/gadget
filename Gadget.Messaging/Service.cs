using System;
using System.Collections.Generic;

namespace Gadget.Messaging
{
    public enum Status
    {
        Healthy,
        Unhealthy
    }

    public class Foo
    {
        public void Bar()
        {
            var service = new Service();
            service
                .AddHandler(s => SendMail(s.Name))
                .AddHandler(Console.WriteLine)
                .AddHandler(s => SendDiscordNotification(s.Name));
        }
    }

    public class Service
    {
        public string Name { get; set; }
        public string Status { get; set; }

        private readonly IDictionary<Status, ICollection<Action<Service>>> _actions =
            new Dictionary<Status, ICollection<Action<Service>>>();

        public Service AddHandler(Action<Service> action)
        {
            if (_actions[Messaging.Status.Healthy] == null)
            {
                _actions[Messaging.Status.Healthy] = new List<Action<Service>>();
            }

            _actions[Messaging.Status.Healthy].Add(action);
            return this;
        }

        private void Handle()
        {
            foreach (var action in _actions[Messaging.Status.Healthy])
            {
                action.Invoke(this);
            }
        }
    }
}