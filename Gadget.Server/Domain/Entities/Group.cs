using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public void RemoveResource(string resourceName)
        {
            var resource = _resources.FirstOrDefault(r => r.Name.ToLower() == resourceName);
            if (resource is null)
            {
                throw new Exception("Requested resource could not be found");
            }

            if (!_resources.Contains(resource))
            {
                return;
            }

            _resources.Remove(resource);
        }
    }
}