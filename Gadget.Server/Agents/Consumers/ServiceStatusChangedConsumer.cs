using System;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events;
using Gadget.Messaging.SignalR;
using Gadget.Server.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly IHubContext<GadgetHub> _hub;
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;
        private readonly GadgetContext _context;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger, IHubContext<GadgetHub> hub,
            GadgetContext context)
        {
            _logger = logger;
            _hub = hub;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            var agentName = context.Message.Agent;
            var service = context.Message.Name;
            var newStatus = context.Message.Status;

            var agent = await _context.Agents
                .Include(a => a.Services)
                .FirstOrDefaultAsync(a => a.Name == agentName);
            if (agent == null)
            {
                throw new ApplicationException($"Agent {agentName} is not registered");
            }

            agent.ChangeServiceStatus(service, newStatus);
            await _context.SaveChangesAsync();
            await _hub.Clients.Group("dashboard").SendAsync("ServiceStatusChanged", new ServiceDescriptor
            {
                Agent = context.Message.Agent,
                Name = context.Message.Name,
                Status = context.Message.Status
            });
            _logger.LogInformation(
                $"Agent {context.Message.Agent} Svc {context.Message.Name} Status {context.Message.Status}");
        }
    }
}