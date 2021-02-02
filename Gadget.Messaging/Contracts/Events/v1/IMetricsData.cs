namespace Gadget.Messaging.Contracts.Events.v1
{
    public interface IMetricsData
    {
        string Agent {get;}
        int CpuPercentUsage {get;}
        float MemoryFree {get;}
        float MemoryTotal {get;}
        int DiscTotal {get;}
        int DiscOccupied { get;}
        int ServicesCount {get;}
        int ServicesRunning {get;}
    }
}
