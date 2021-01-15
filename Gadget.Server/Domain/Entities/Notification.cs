using System;

namespace Gadget.Server.Domain.Entities
{
    /// <summary>
    /// Notification encapsulates data needed to notify other parties about service status changed it is intended only for this usage
    /// </summary>
    public class Notification
    {
        public Guid Id { get; }
        public string Agent { get; }
        public string Service { get; }
        public string Status { get; }

        public Notification(string agent, string service, string status)
        {
            Id = Guid.NewGuid();
            Agent = agent ?? throw new ArgumentNullException(nameof(agent));
            Service = service ?? throw new ArgumentNullException(nameof(service));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }
    }
}