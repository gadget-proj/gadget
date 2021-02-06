using System.Threading.Tasks;
using Gadget.Common;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Domain.Enums;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Persistence;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Commands
{
    public class CreateWebhookCommand : ICommand
    {
        public string AgentName { get; set; }
        public string ServiceName { get; set; }
        public string Receiver { get; set; }
    }

    public class CreateWebhookHandler : IHandler<CreateWebhookCommand>
    {
        private readonly NotificationsContext _notificationsContext;
        private readonly ILogger<CreateWebhookHandler> _logger;

        public CreateWebhookHandler(NotificationsContext notificationsContext, ILogger<CreateWebhookHandler> logger)
        {
            _notificationsContext = notificationsContext;
            _logger = logger;
        }

        public async Task Handle(CreateWebhookCommand t)
        {
            _logger.LogInformation("CreateWebhookHandler");
            var notification = new Notification(t.AgentName, t.ServiceName);
            var notifier = new Notifier(t.AgentName, t.ServiceName, t.Receiver, NotifierType.Discord);
            notification.AddNotifier(notifier);
            await _notificationsContext.Notifications.AddAsync(notification);
            await _notificationsContext.SaveChangesAsync();
        }
    }
}