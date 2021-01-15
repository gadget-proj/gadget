using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging.SignalR;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Notifications.Services
{
    public class NotificationsService : BackgroundService
    {
        private readonly Channel<Notification> _notifications;
        private readonly ILogger<NotificationsService> _logger;
        private readonly IHubContext<GadgetHub> _hub;

        public NotificationsService(Channel<Notification> notifications, ILogger<NotificationsService> logger,
            IHubContext<GadgetHub> hub)
        {
            _notifications = notifications;
            _logger = logger;
            _hub = hub;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _notifications.Reader.WaitToReadAsync(stoppingToken);
                var notification = await _notifications.Reader.ReadAsync(stoppingToken);
                await _hub.Clients.Group("dashboard").SendAsync("ServiceStatusChanged", new ServiceDescriptor
                {
                    Agent = notification.Agent,
                    Name = notification.Service,
                    Status = notification.Status
                }, stoppingToken);
                _logger.LogInformation($"NotificationId : {notification.Id}, new status : {notification.Status} for service {notification.Service}");
            }
        }
    }
}