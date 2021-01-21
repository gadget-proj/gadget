using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gadget.Server.Domain.Entities
{
    public class Agent
    {
        public Agent(string name, string address)
        {
            Name = name;
            Address = address;
            Id = Guid.NewGuid();
        }

        private Agent()
        {
        }

        public Guid Id { get;  }
        private readonly ICollection<Service> _services = new List<Service>();
        public string Name { get; }
        public IEnumerable<Service> Services => _services.ToImmutableList();
        public string Address { get;  }

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
            var service = _services.FirstOrDefault(s => s.Name == serviceName);
            if (service is null)
            {
                throw new ApplicationException($"Service {serviceName} could not be found on agent {Name}");
            }
            
            service.ChangeStatus(newStatus);
        }
    }
}