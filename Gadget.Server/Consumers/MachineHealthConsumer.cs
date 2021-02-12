using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Messaging.SignalR.v1;
using Gadget.Server.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MachineHealthConsumer : IConsumer<IMetricsData>
    {
        private readonly ILogger<MachineHealthConsumer> _logger;
        private Dictionary<string, DateTime> _agentLastChecks;
        public MachineHealthConsumer(ILogger<MachineHealthConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IMetricsData> context)
        {
            // await _hub.Clients.Group("dashboard").SendAsync("MachineHealthReceived", new MachineHealthData
            // {
            //     Agent = context.Message.Agent,
            //     CpuPercentUsage = context.Message.CpuPercentUsage,
            //     MemoryFree = context.Message.MemoryFree,
            //     MemoryTotal = context.Message.MemoryTotal,
            //     DiscTotal = context.Message.DiscTotal,
            //     DiscOccupied = context.Message.DiscOccupied,
            //     ServicesCount = context.Message.ServicesCount,
            //     ServicesRunning = context.Message.ServicesRunning
            // });
        }
    }
}