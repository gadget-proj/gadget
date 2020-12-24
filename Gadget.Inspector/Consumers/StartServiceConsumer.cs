using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class StartServiceConsumer : IConsumer<IStartService>
    {
        private readonly ILogger<StartServiceConsumer> _logger;

        public StartServiceConsumer(ILogger<StartServiceConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStartService> context)
        {
            _logger.LogInformation($"Received {context.Message.GetType()}");
        }
    }
}