using System.Threading.Tasks;
using Gadget.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents.Consumers
{
    public class ServiceStatusChanged : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChanged> _logger;

        public ServiceStatusChanged(ILogger<ServiceStatusChanged> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            _logger.LogInformation($"{context.Message.GetType()} received");
        }
    }
}