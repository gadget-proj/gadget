using System;

namespace Gadget.Server.Domain.Entities
{
    public class ServiceEvent
    {
        public Guid Id { get; }
        public string Status { get; }
        public DateTime CreatedAt { get; }

        private ServiceEvent()
        {
        }

        public ServiceEvent(string status)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            CreatedAt = DateTime.UtcNow;
        }
    }
}