using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Messaging.SignalR.v1;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Hubs;
using Gadget.Notifications.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Consumers
{
    public class ServiceStatusChangedConsumer : IConsumer<IServiceStatusChanged>
    {
        private readonly ILogger<ServiceStatusChangedConsumer> _logger;
        private readonly IHubContext<NotificationsHub> _hub;
        private readonly ChannelWriter<Notification> _channel;
        private readonly NotificationsContext _notificationsContext;

        public ServiceStatusChangedConsumer(ILogger<ServiceStatusChangedConsumer> logger,
            IHubContext<NotificationsHub> hub, Channel<Notification> channel, NotificationsContext notificationsContext)
        {
            _logger = logger;
            _hub = hub;
            _notificationsContext = notificationsContext;
            _channel = channel.Writer;
        }

        public async Task Consume(ConsumeContext<IServiceStatusChanged> context)
        {
            _logger.LogInformation(
                $"Service {context.Message.Name} has changed its status to {context.Message.Status}");
            try
            {
                await _hub.Clients.Group("dashboard").SendAsync("ServiceStatusChanged", new ServiceDescriptor
                {
                    Agent = context.Message.Agent,
                    Name = context.Message.Name,
                    Status = context.Message.Status
                }, context.CancellationToken);
                _logger.LogInformation("Trying to enqueue webhook notification");
                
                var agent = await _notificationsContext.Services
                    .Include(s=>s.Webhooks)
                    .FirstOrDefaultAsync(s =>
                    s.Agent == context.Message.Agent && s.Name == context.Message.Name);
                
                if (agent is null)
                {
                    _logger.LogInformation("There are not webhooks registered for this event, skipping");
                    return;
                }

                foreach (var agentWebhook in agent.Webhooks)
                {
                    await _channel.WriteAsync(new Notification(context.Message.Agent, context.Message.Name,
                        context.Message.Status, agentWebhook.Uri));
                    _logger.LogInformation("Enqueued webhook notification");
                }
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