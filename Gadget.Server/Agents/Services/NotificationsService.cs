using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Server.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents.Services
{
    public class NotificationsService : BackgroundService
    {
        private readonly Channel<Notification> _notifications;
        private readonly ILogger<NotificationsService> _logger;

        public NotificationsService(Channel<Notification> notifications, ILogger<NotificationsService> logger)
        {
            _notifications = notifications;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _notifications.Reader.WaitToReadAsync(stoppingToken);
                var notification = await _notifications.Reader.ReadAsync(stoppingToken);
                _logger.LogInformation($"NotificationId : {notification.Id}, message : {notification.Message}");
            }
        }
    }
}