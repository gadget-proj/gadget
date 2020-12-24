using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class GetAgentHealthConsumer : IConsumer<IGetAgentHealth>
    {
        private readonly ILogger<GetAgentHealthConsumer> _logger;

        public GetAgentHealthConsumer(ILogger<GetAgentHealthConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IGetAgentHealth> context)
        {
            _logger.LogInformation($"Received {context.Message.GetType()}");
        }
    }
}