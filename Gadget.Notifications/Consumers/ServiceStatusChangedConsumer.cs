using System;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events;
using Gadget.Messaging.SignalR;
using Gadget.Notifications.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;
        private readonly IHubContext<NotificationsHub> _hub;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger,
            IHubContext<NotificationsHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            _logger.LogInformation(
                $"Service {context.Message.Agent} has changed its status to {context.Message.Status}");
            try
            {
                await _hub.Clients.Group("dashboard").SendAsync("ServiceStatusChanged", new ServiceDescriptor
                {
                    Agent = context.Message.Agent,
                    Name = context.Message.Name,
                    Status = context.Message.Status
                }, context.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return;
            }

            _logger.LogInformation("Invoked hub notification");
        }
    }
}