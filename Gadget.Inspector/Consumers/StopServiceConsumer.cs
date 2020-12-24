using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class StopServiceConsumer : IConsumer<IStopService>
    {
        private readonly ILogger<StopServiceConsumer> _logger;

        public StopServiceConsumer(ILogger<StopServiceConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStopService> context)
        {
            _logger.LogInformation($"Received {context.Message.GetType()}");
        }
    }
}