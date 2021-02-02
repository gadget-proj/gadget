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

        public Guid Id { get; }
        public string Name { get; }
        public string Address { get; }
        private readonly ICollection<Service> _services = new List<Service>();
        public IEnumerable<Service> Services => _services.ToImmutableList();
        private readonly ICollection<Uri> _webhooks = new HashSet<Uri>();
        public ICollection<Uri> Webhooks => _webhooks.ToImmutableList();

        private void AddService(Service service)
        {
            _services.Add(service);
        }

        public void AddServices(IEnumerable<Service> services)
        {
            foreach (var service in services) AddService(service);
        }

        private void AddWebhook(Uri webhook)
        {
            _webhooks.Add(webhook);
        }

        public void AddWebhooks(IEnumerable<Uri> webhooks)
        {
            foreach (var webhook in webhooks)
            {
                AddWebhook(webhook);
            }
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