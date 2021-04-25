using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gadget.Server.Domain.Entities
{
    public class Agent
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Address { get; }
        private readonly ICollection<Service> _services = new HashSet<Service>();
        public IEnumerable<Service> Services => _services.ToImmutableList();
        public bool Alive { get; private set; }
        public DateTime LastAlive { get; set; }

        private Agent()
        {
        }

        public Agent(string name, string address)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        private void AddService(Service service)
        {
            _services.Add(service);
        }

        public void AddServices(IEnumerable<Service> services)
        {
            foreach (var service in services) AddService(service);
        }

        public void ChangeServiceStatus(string serviceName, string newStatus)
        {
            var service = _services.FirstOrDefault(s =>
                string.Equals(s.Name, serviceName, StringComparison.CurrentCultureIgnoreCase));
            if (service is null)
            {
                throw new ApplicationException($"Service {serviceName} could not be found on agent {Name}");
            }

            service.ChangeStatus(newStatus);
        }

        public void MarkNotAvailable()
        {
            Alive = false;
            LastAlive = DateTime.UtcNow;
        }
    }
}