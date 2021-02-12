using System.Threading;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Domain.Enums;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Persistence;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Services.Interfaces
{
    public interface INotificationsService
    {
        Task RegisterNotification(string agentName, string serviceName, CancellationToken cancellationToken);

        Task RegisterNotifier(string agentName, string serviceName, string receiver, NotifierType notifierType,
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
    }
}