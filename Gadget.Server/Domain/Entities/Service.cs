using System;
using System.Collections.Generic;

namespace Gadget.Server.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Status { get; private set; }
        public string LogOnAs { get; }
        public string Description { get; }
        public Agent Agent { get; }
        public bool Restart { get; set; } = false;
        public readonly ICollection<ServiceEvent> Events = new List<ServiceEvent>();

        private Service()
        {
        }

        public Service(string name, string status, Agent agent, string logOnAs, string description)
        {
            Name = name;
            Status = status;
            Agent = agent;
            LogOnAs = logOnAs;
            Description = description;
            Id = Guid.NewGuid();
        }

        public void ChangeStatus(string status)
        {
            Status = status;
            Events.Add(new ServiceEvent(status));
        }
    }
}