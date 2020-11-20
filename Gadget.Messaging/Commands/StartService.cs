namespace Gadget.Messaging.Commands
{
    public class StartService : IGadgetMessage
    {
        public string Agent { get; set; }
        public string ServiceName { get; set; }
    }
}