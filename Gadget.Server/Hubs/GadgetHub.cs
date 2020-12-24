using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using Gadget.Server.Domain.Entities;
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

        public Task RegisterDashboard(RegisterNewDashboard registerNewDashboard)
        {
            _logger.LogInformation($"{Context.ConnectionId} joining dashboard group");
            var connectionId = Context.ConnectionId;
            Groups.AddToGroupAsync(connectionId, "dashboard");
            _logger.LogInformation($"{Context.ConnectionId} successfully joined dashboard group");
            return Task.CompletedTask;
        }
    }
}