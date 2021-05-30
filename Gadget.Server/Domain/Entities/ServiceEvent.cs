using System;
using Gadget.Server.Domain.Enums;

namespace Gadget.Server.Domain.Entities
{
    public class ServiceEvent
    {
        private ServiceEvent()
        {
        }

        public ServiceEvent(ServiceStatus status)
        {
            Status = status;
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; }
        public ServiceStatus Status { get; }
        public DateTime CreatedAt { get; }
        public Service Service { get; }
    }
}