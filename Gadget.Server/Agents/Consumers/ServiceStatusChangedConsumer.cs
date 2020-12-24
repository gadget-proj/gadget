using System.Threading.Tasks;
using Gadget.Messaging.Events;
using Gadget.Server.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly IHubContext<GadgetHub> _hub;
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger, IHubContext<GadgetHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            await _hub.Clients.Group("dashboard").SendAsync("ServiceStatusChanged", context.Message);
            _logger.LogInformation($"{context.Message.GetType()} received");
        }
    }
}