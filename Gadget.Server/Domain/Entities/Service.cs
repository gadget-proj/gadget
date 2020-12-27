using System;
using System.Collections.Generic;

namespace Gadget.Server.Domain.Entities
{
    public class Service
    {
        public Service(string name, string status, Agent agent)
        {
            Name = name;
            Status = status;
            Agent = agent;
            Id = Guid.NewGuid();
        }

        private Service()
        {
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Status { get; set; }

        public Agent Agent { get; }

        public static IEqualityComparer<Service> NameComparer { get; } = new NameEqualityComparer();

        private sealed class NameEqualityComparer : IEqualityComparer<Service>
        {
            public bool Equals(Service x, Service y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(Service obj)
            {
                return obj.Name != null ? obj.Name.GetHashCode() : 0;
            }
        }
    }
}