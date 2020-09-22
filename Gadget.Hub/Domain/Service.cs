using System;

namespace Gadget.Hub.Domain
{
    public class Service
    {
        public Guid Id { get; }
        public string Name { get; }

        public Service(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
