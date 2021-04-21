using System;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;
        private readonly GadgetContext _context;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger, GadgetContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            var agentName = context.Message.Agent;
            var service = context.Message.Name;
            var newStatus = context.Message.Status;
            var id = context.CorrelationId;
            
            var agent = await _context.Agents
                .Include(a => a.Services)
                .ThenInclude(s => s.Events.Take(1))
                .FirstOrDefaultAsync(a => a.Name == agentName);
            if (agent == null)
            {
                throw new ApplicationException($"Agent {agentName} is not registered");
            }

            var changedService = agent.Services.FirstOrDefault(s => s.Name == service);
            if (changedService is null)
            {
                return;
            }

            var newEvent = new ServiceEvent(newStatus);
            changedService.Events.Add(newEvent);
            
            if (changedService.Config.Restart)
            {
                _logger.LogCritical("restart!");
            }

            agent.ChangeServiceStatus(service, newStatus);
            _context.Agents.Update(agent);
            await _context.SaveChangesAsync();
            _logger.LogInformation(
                $"Agent {context.Message.Agent} Svc {context.Message.Name} Status {context.Message.Status}");
        }
    }
}