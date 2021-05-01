using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Domain.Entities.Events;
using Gadget.Server.Domain.Enums;
using Gadget.Server.Persistence;
using Gadget.Server.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Action = Gadget.Server.Domain.Enums.Action;

namespace Gadget.Server.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;
        private readonly GadgetContext _context;
        private readonly ChannelWriter<IServiceStatusChanged> _events;
        private readonly IAgentsService _agentsService;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger, GadgetContext context,
            Channel<IServiceStatusChanged> events, IAgentsService agentsService)
        {
            _logger = logger;
            _context = context;
            _agentsService = agentsService;
            _events = events.Writer;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            var agentName = context.Message.Agent;
            var service = context.Message.Name;
            var newStatus = Enum.Parse<ServiceStatus>(context.Message.Status);
            var id = context.CorrelationId;
            var @event = ParseEvent(context.Message);
            var svc2 = await _context.Services
                .Include(s => s.Config)
                .ThenInclude(c=>c.Actions)
                .Include(s => s.Events)
                .FirstOrDefaultAsync(s => s.Name.ToLower() == service.ToLower());

            await _events.WriteAsync(context.Message);

            if (svc2 is null)
            {
                return;
            }

            var action = svc2.Act(@event);
            switch (action)
            {
                case Action.Stop:
                    _logger.LogInformation("Trying to stop service");
                    await _agentsService.StopService(agentName, service);
                    break;
                case Action.Start:
                    _logger.LogInformation("Trying to start service");
                    await _agentsService.StartService(agentName, service);
                    break;
                case Action.Pass:
                    _logger.LogInformation("Passing on this event");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newEvent = new ServiceEvent(newStatus);
            svc2.Events.Add(newEvent);
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Name.ToLower() == agentName.ToLower());
            agent.ChangeServiceStatus(service, newStatus);
            _context.Agents.Update(agent);
            await _context.SaveChangesAsync();
            _logger.LogInformation(
                $"Agent {context.Message.Agent} Svc {context.Message.Name} Status {context.Message.Status}");
        }

        private Event ParseEvent(IServiceStatusChanged serviceStatusChanged)
        {
            var @event = new Event
            {
                ServiceStatus = Enum.Parse<ServiceStatus>(serviceStatusChanged.Status),
                Date = DateTime.UtcNow
            };
            return @event;
        }
    }
}