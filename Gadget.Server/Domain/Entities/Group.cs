using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gadget.Server.Domain.Entities
{
    public class Group 
    {
        public Guid Id { get; }
        public string Name { get; }
        private readonly ICollection<Service> _resources = new HashSet<Service>();
        public IEnumerable<Service> Resources => _resources.ToImmutableList();

        public Group(string name)
        {
            Name = name;
        }

        public void AddResource(Service service)
        {
            _resources.Add(service);
        }
    }
}