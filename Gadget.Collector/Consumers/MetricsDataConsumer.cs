using System;
using System.Threading.Tasks;
using Gadget.Collector.Metrics;
using Gadget.Collector.Persistence;
using Gadget.Messaging.Contracts.Events.v1;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Collector.Consumers
{
    public class MetricsDataConsumer : IConsumer<IMetricsData>
    {
        private readonly ILogger<MetricsDataConsumer> _logger;
        private readonly CollectorContext _context;

        public MetricsDataConsumer(ILogger<MetricsDataConsumer> logger, CollectorContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IMetricsData> context)
        {
            var agentNormalized = context.Message.Agent.Replace("-", "");
            var basicMetric = new BasicMetric(Guid.NewGuid(), agentNormalized, context.Message.CpuPercentUsage,
                context.Message.MemoryFree, context.Message.MemoryTotal, context.Message.DiscTotal,
                context.Message.DiscOccupied, context.Message.ServicesCount, context.Message.ServicesRunning,
                DateTime.UtcNow);
            await _context.BasicMetrics.AddAsync(basicMetric);
            await _context.SaveChangesAsync();
            _logger.LogInformation("new metrics data");
        }
    }
}