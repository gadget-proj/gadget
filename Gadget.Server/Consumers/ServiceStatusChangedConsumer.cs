using System;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
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
            
            _logger.LogInformation($"Service {service} on an agent {agentName} changed its status to {newStatus}");
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
                _logger.LogError($"Service {service} on agent {agentName} is not registered on this server");
                return;
            }

            var newEvent = new ServiceEvent(newStatus);
            changedService.Events.Add(newEvent);

            if (changedService.Restart && newStatus == "Stopped")
            {
                //Restart
                _logger.LogInformation($"Trying to restart service {service} on an agent {agentName}");
                await context.Publish<IStartService>(new
                {
                    ServiceName = service,
                    Agent = agentName
                }, ctx => { ctx.SetRoutingKey(service); });
            }

            agent.ChangeServiceStatus(service, newStatus);
            _context.Agents.Update(agent);
            await _context.SaveChangesAsync();
        }
    }
}