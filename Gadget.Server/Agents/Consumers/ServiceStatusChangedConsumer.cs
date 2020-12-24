using System.Threading.Tasks;
using Gadget.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            _logger.LogInformation($"{context.Message.GetType()} received");
        }
    }
}