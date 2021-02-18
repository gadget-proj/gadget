using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Domain.Enums;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Dto;
using Gadget.Notifications.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Services.Interfaces
{
    public interface INotificationsService
    {
        Task RegisterNotification(string agentName, string serviceName, CancellationToken cancellationToken);

        Task RegisterNotifier(string agentName, string serviceName, string receiver, NotifierType notifierType,
            CancellationToken cancellationToken);

        Task<IEnumerable<NotificationDto>> GetWebhooks(string agentName, string serviceName,
            CancellationToken cancellationToken);
    }

    public class NotificationsService : INotificationsService
    {
        private readonly ILogger<NotificationsService> _logger;
        private readonly NotificationsContext _notificationsContext;

        public NotificationsService(ILogger<NotificationsService> logger, NotificationsContext notificationsContext)
        {
            _logger = logger;
            _notificationsContext = notificationsContext;
        }

        public async Task RegisterNotification(string agentName, string serviceName,
            CancellationToken cancellationToken)
        {
            var notification = new Notification(agentName, serviceName);
            await _notificationsContext.Notifications.AddAsync(notification, cancellationToken);
            await _notificationsContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RegisterNotifier(string agentName, string serviceName, string receiver,
            NotifierType notifierType,
            CancellationToken cancellationToken)
        {
            var notification = new Notification(agentName, serviceName);
            var notifier = new Notifier(agentName, serviceName, receiver, NotifierType.Discord);
            notification.AddNotifier(notifier);
            await _notificationsContext.Notifications.AddAsync(notification, cancellationToken);
            await _notificationsContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<NotificationDto>> GetWebhooks(string agentName, string serviceName,
            CancellationToken cancellationToken)
        {
            var notifications = await _notificationsContext.Notifications
                .Where(n => n.Agent == agentName && n.Service == serviceName)
                .Include(n => n.Notifiers)
                .ToListAsync(cancellationToken);
            return notifications.Select(n => new NotificationDto
            {
                Agent = n.Agent,
                Id = n.Id,
                Notifiers = n.Notifiers.Select(nn=> new NotifierDto
                {
                    Receiver = nn.Receiver,
                    CreatedAt = nn.CreatedAt,
                    Type = nn.NotifierType.ToString(),
                }),
                Service = n.Service
            });
        }
    }
}