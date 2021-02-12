using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Hubs
{
    public class NotificationsHub : Hub<NotificationsHub>
    {
        private readonly ILogger<NotificationsHub> _logger;

        public NotificationsHub(ILogger<NotificationsHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"{Context.ConnectionId} connected");
            var connectionId = Context.ConnectionId;
            Groups.AddToGroupAsync(connectionId, "dashboard");
            _logger.LogInformation($"{Context.ConnectionId} successfully joined dashboard group");
            return Task.CompletedTask;
        }
    }
}