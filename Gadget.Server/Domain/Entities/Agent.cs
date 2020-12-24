using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gadget.Server.Domain.Entities
{
    public class Agent
    {
        private readonly ICollection<Service> _services;

        public Agent(string name)
        {
            Name = name;
            _services = new HashSet<Service>();
        }

        public string Name { get; }
        public IEnumerable<Service> Services => _services.ToImmutableList();

        public void AddService(Service service)
        {
            _services.Add(service);
        }

        public void AddServices(IEnumerable<Service> services)
        {
            foreach (var service in services) AddService(service);
        }
    }
}