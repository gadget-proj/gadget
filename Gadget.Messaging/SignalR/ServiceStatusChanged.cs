namespace Gadget.Messaging.SignalR
{
    public class ServiceStatusChanged
    {
        public string AgentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}