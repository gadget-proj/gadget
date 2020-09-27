using System;

namespace Gadget.Messaging
{
    public class ServiceStatusChanged
    {
        public Guid AgentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}