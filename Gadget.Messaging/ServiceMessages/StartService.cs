using System;

namespace Gadget.Messaging.ServiceMessages
{
    public class StartService
    {
        public Guid AgentId { get; set; }
        public string ServiceName { get; set; }
    }
}