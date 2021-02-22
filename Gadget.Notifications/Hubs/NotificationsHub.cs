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
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"{connectionId} connected to the hub");
            Groups.AddToGroupAsync(connectionId, "dashboard");
            _logger.LogInformation($"{connectionId} successfully joined dashboard group");
            return Task.CompletedTask;
        }
    }
}