namespace Gadget.Messaging.Commands
{
    public class GetAgentHealth : IGadgetMessage
    {
        public string Agent { get; set; }

        public string MachineName { get; set; }
        public int CpuPercentUsage { get; set; }

        public float MemoryFree { get; set; }
        public float MemoryTotal { get; set; }

        public float DiscFree { get; set; }
        public float DiscTotal { get; set; }
    }
}