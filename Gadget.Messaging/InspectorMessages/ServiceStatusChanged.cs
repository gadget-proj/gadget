using System;

namespace Gadget.Messaging.ServiceMessages
{
    public class ServiceStatusChanged
    {
        public Guid AgentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}