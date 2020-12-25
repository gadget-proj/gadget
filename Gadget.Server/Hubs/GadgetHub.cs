using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Hubs
{
    public class GadgetHub : Hub
    {
        private readonly ILogger<GadgetHub> _logger;

        public GadgetHub(ILogger<GadgetHub> logger)
        {
            _logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return Task.CompletedTask;
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