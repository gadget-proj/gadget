using System;

namespace Gadget.Messaging.Events
{
    public class ServiceStatusChanged
    {
        public Guid AgentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}