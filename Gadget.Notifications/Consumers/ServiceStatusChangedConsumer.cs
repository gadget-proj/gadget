using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            _logger.LogInformation(
                $"Service {context.Message.Agent} has changed its status to {context.Message.Status}");
            return Task.CompletedTask;
        }
    }
}