using Gadget.Messaging.Commands;

namespace Gadget.Messaging.Events
{
    public class ServiceStatusChanged : IGadgetMessage
    {
        public string AgentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}