namespace Gadget.Messaging.Commands
{
    public class StopService : IGadgetMessage
    {
        public string Agent { get; set; }
        public string ServiceName { get; set; }
    }
}