using System;

namespace Gadget.Messaging.Commands
{
    public class StartService
    {
        public Guid AgentId { get; set; }
        public string ServiceName { get; set; }
    }
}