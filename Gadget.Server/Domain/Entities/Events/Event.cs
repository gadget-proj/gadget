using System;
using Gadget.Server.Domain.Enums;

namespace Gadget.Server.Domain.Entities.Events
{
    public class Event
    {
        public ServiceStatus ServiceStatus { get; set; }
        public DateTime Date { get; set; }
    }

    public class ServiceStoppedEvent : Event
    {
    }
}