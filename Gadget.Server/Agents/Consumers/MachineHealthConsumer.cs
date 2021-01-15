using Gadget.Messaging.Contracts.Events;
using Gadget.Messaging.SignalR;
using Gadget.Server.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Gadget.Server.Agents.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MachineHealthConsumer : IConsumer<IMetricsData>
    {
        private readonly IHubContext<GadgetHub> _hub;
        private readonly ILogger<MachineHealthConsumer> _logger;

        public MachineHealthConsumer(ILogger<MachineHealthConsumer> logger, IHubContext<GadgetHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public async Task Consume(ConsumeContext<IMetricsData> context)
        {
            await _hub.Clients.Group("dashboard").SendAsync("MachineHealthReceived", new MachineHealthData
            {
                Agent = context.Message.Agent,
                CpuPercentUsage = context.Message.CpuPercentUsage,
                MemoryFree = context.Message.MemoryFree,
                MemoryTotal = context.Message.MemoryTotal,
                DiscTotal = context.Message.DiscTotal,
                DiscOccupied = context.Message.DiscOccupied,
                ServicesCount = context.Message.ServicesCount,
                ServicesRunning = context.Message.ServicesRunning
            });
        }
    }
}