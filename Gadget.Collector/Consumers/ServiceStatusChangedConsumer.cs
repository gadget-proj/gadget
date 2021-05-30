using System;
using System.Threading.Tasks;
using Gadget.Collector.Events;
using Gadget.Collector.Persistence;
using Gadget.Messaging.Contracts.Events.v1;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Collector.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly CollectorContext _context;
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;

        public ServiceStatusChangedConsumer(CollectorContext context, ILogger<ServiceStatusChangedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            var @event = new ServiceStatusChangedEvent(Guid.NewGuid(), context.Message.Agent, context.Message.Name,
                context.Message.Status, context.Message.Date);
            _logger.LogInformation($"Service {context.Message.Name} changed to {context.Message.Status}");
            try
            {
                await _context.StatusChangedEvents.AddAsync(@event);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}