using System;
using System.Threading.Tasks;
using Gadget.Collector.Events;
using Gadget.Collector.Persistence;
using Gadget.Messaging.Contracts.Events.v1;
using MassTransit;

namespace Gadget.Collector.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly CollectorContext _context;

        public ServiceStatusChangedConsumer(CollectorContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            var @event = new ServiceStatusChangedEvent(Guid.NewGuid(), context.Message.Agent, context.Message.Name,
                context.Message.Status, context.Message.Date);
            await _context.StatusChangedEvents.AddAsync(@event);
            await _context.SaveChangesAsync();
        }
    }
}