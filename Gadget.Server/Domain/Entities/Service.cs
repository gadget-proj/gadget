using System.Collections.Generic;
using Gadget.Server.Domain.Enums;

namespace Gadget.Server.Domain.Entities
{
    public class Service
    {
        public string Name { get; }
        public string Status { get; set; }

        public Service(string name, string status)
        {
            Name = name;
            Status = status;
        }

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
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<Service> NameComparer { get; } = new NameEqualityComparer();
    }
}