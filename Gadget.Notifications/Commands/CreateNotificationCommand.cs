using System.Threading.Tasks;
using Gadget.Common;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Persistence;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Commands
{
    public class CreateNotificationCommand : ICommand
    {
        public string AgentName { get; set; }
        public string ServiceName { get; set; }
    }

    public class CreateNotificationHandler : IHandler<CreateNotificationCommand>
    {
        private readonly ILogger<CreateNotificationHandler> _logger;
        private readonly NotificationsContext _notificationsContext;

        public CreateNotificationHandler(ILogger<CreateNotificationHandler> logger,
            NotificationsContext notificationsContext)
        {
            _logger = logger;
            _notificationsContext = notificationsContext;
        }

        public async Task Handle(CreateNotificationCommand t)
        {
            _logger.LogInformation("CreateNotificationHandler");
            var notification = new Notification(t.AgentName, t.ServiceName);
            await _notificationsContext.Notifications.AddAsync(notification);
            await _notificationsContext.SaveChangesAsync();
        }
    }
}