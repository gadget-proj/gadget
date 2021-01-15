using System;

namespace Gadget.Server.Domain.Entities
{
    public class ServiceEvent
    {
        public int Id { get; private set; }
        public string Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

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