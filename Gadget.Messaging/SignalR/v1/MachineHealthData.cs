namespace Gadget.Messaging.SignalR.v1
{
    public class MachineHealthData
    {
        public string Agent { get; set; }
        public int CpuPercentUsage { get; set; }
        public float MemoryFree { get; set; }
        public float MemoryTotal { get; set; }
        public int DiscTotal { get; set; }
        public int DiscOccupied { get; set; }
        public int ServicesCount { get; set; }
        public int ServicesRunning { get; set; }
    }
}
