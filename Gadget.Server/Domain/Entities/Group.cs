using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gadget.Server.Domain.Entities
{
    public class Group : Resource
    {
        public string Name { get; }
        private readonly ICollection<Resource> _resources = new HashSet<Resource>();
        public IEnumerable<Resource> Resources => _resources.ToImmutableList();

        public Group(string name)
        {
            Name = name;
        }

        public void AddResource(Resource resource)
        {
            _resources.Add(resource);
        }
    }
}