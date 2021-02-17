﻿using System;

namespace Gadget.Server.Domain.Entities
{
    public class ServiceEvent
    {
        private ServiceEvent()
        {
        }

        public ServiceEvent(string status, Guid serviceId)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; }
        public string Status { get; }
        public DateTime CreatedAt { get; }
        public Service Service { get; }
    }
}