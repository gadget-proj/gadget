using System;
using System.Collections.Generic;
using Gadget.Server.Domain.Entities.Events;
using Gadget.Server.Domain.Enums;
using Action = Gadget.Server.Domain.Enums.Action;

namespace Gadget.Server.Domain.Entities
{
    public class Service
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public Guid Id { get; }
        public string Name { get; }
        public string LogOnAs { get; }
        public string Description { get; }
        public ServiceStatus Status { get; private set; }
        public Agent Agent { get; }
        public Config Config { get; private set; }
        public readonly ICollection<ServiceEvent> Events = new List<ServiceEvent>();
        public static IEqualityComparer<Service> NameComparer { get; } = new NameEqualityComparer();

        private Service()
        {
        }

        public Service(string name, ServiceStatus status, Agent agent, string logOnAs, string description)
        {
            Name = name;
            Status = status;
            Agent = agent;
            LogOnAs = logOnAs;
            Description = description;
        }

        public void ChangeStatus(ServiceStatus status)
        {
            Status = status;
            Events.Add(new ServiceEvent(status));
        }

        public void ApplyConfig(Config config)
        {
            Config = config;
        }

        public Action Act(Event @event)
        {
            return Config?.Action(@event.ServiceStatus) ?? Action.Pass;
        }

        private sealed class NameEqualityComparer : IEqualityComparer<Service>
        {
            public bool Equals(Service x, Service y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(Service obj)
            {
                return obj.Name != null ? obj.Name.GetHashCode() : 0;
            }
        }
    }
}