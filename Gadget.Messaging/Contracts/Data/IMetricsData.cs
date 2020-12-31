namespace Gadget.Messaging.Contracts.Data
{
    public interface IMetricsData
    {
        string MachineName {get;}
        int CpuPercentUsage {get;}
        float MemoryFree {get; }
        float MemoryTotal {get;}
        int DiscTotal { get; set; }
        int DiscOccupied { get; set; }
    }
}
