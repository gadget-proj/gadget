using System;

namespace Gadget.Messaging.ServiceMessages
{
    public class StopService
    {
        public Guid AgentId { get; set; }
        public string ServiceName { get; set; }
    }
}
