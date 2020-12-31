namespace Gadget.Messaging.Contracts.Data
{
    public interface IMetricsData
    {
        string Agent {get;}
        int CpuPercentUsage {get;}
        float MemoryFree {get;}
        float MemoryTotal {get;}
        int DiscTotal {get;}
        int DiscOccupied { get;}
    }
}
