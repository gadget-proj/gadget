using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Hubs
{
    public class NotificationsHub : Hub<NotificationsHub>
    {
        private readonly ILogger<NotificationsHub> _logger;
        private readonly ISubscriptionsManager _subscriptionsManager;

        public NotificationsHub(ILogger<NotificationsHub> logger, ISubscriptionsManager subscriptionsManager)
        {
            _logger = logger;
            _subscriptionsManager = subscriptionsManager;
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"{connectionId} connected to the hub");
            Groups.AddToGroupAsync(connectionId, "dashboard");
            _logger.LogInformation($"{connectionId} successfully joined dashboard group");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Selector should be a valid regex expression used to match incoming events
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task Subscribe(string selector)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, selector);
            await _subscriptionsManager.Add(selector);
            _logger.LogInformation($"Added {Context.ConnectionId} to subs group {selector}");
        }
    }
}