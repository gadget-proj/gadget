using System;
using System.Collections.Generic;

namespace Gadget.Notifications.Domain.Entities
{
    public class Agent
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Address { get; }

        private readonly ICollection<Service> _services = new HashSet<Service>();

        public Agent(Guid id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }
    }
}