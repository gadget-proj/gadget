using System;

namespace Gadget.Collector.Metrics
{
    public record BasicMetric(Guid Id, string Agent, int CpuPercentUsage, float MemoryFree, float MemoryTotal,
        int DiscTotal,
        int DiscOccupied, int ServicesCount, int ServicesRunning, DateTime Date);
}