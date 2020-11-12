using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Hubs
{
    public class GadgetHub : Hub
    {
        private readonly IDictionary<Guid, ICollection<Service>> _agents;
        private readonly IDictionary<string, Guid> _connectedClients;
        private readonly ILogger<GadgetHub> _logger;

        public GadgetHub(IDictionary<Guid, ICollection<Service>> agents, IDictionary<string, Guid> connectedClients,
            ILogger<GadgetHub> logger)
        {
            _agents = agents;
            _connectedClients = connectedClients;
            _logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            _connectedClients.Remove(connectionId);
            _logger.LogInformation($"Client {connectionId} has disconnected");
            return Task.CompletedTask;
        }

        public Task RegisterMachineReport(RegisterNewAgent registerMachineReport)
        {
            _logger.LogInformation($"Received new machine report from agent {registerMachineReport.AgentId}");
            _agents[registerMachineReport.AgentId] = registerMachineReport.Services.ToList();
            foreach (var service in registerMachineReport.Services)
            {
                _logger.LogInformation($"Service name : {service.Name} status : {service.Status}");
            }

            return Task.CompletedTask;
        }

        //TODO Remove from group on disconnect
        public Task RegisterDashboard(RegisterNewDashboard registerNewDashboard)
        {
            var connectionId = Context.ConnectionId;
            Groups.AddToGroupAsync(connectionId, "dashboard");
            return Task.CompletedTask;
        }

        public Task Register(RegisterNewAgent registerNewAgent)
        {
            var connectionId = Context.ConnectionId;
            var agentId = registerNewAgent.AgentId;
            if (_agents.ContainsKey(agentId)) return Task.CompletedTask;
            _agents[agentId] = registerNewAgent.Services.ToList();
            _connectedClients[connectionId] = agentId;
            return Task.CompletedTask;
        }

        public Task ServiceStatusChanged(ServiceStatusChanged serviceStatusChanged)
        {
            var connectionId = Context.ConnectionId;
            if (!_connectedClients.TryGetValue(connectionId, out var agentId))
            {
                return Task.CompletedTask;
            }

            if (!_agents.TryGetValue(agentId, out var services))
            {
                return Task.CompletedTask;
            }

            var serviceName = serviceStatusChanged.Name;
            var serviceStatus = serviceStatusChanged.Status;
            var service = services.Single(s =>
                string.Equals(s.Name, serviceName, StringComparison.CurrentCultureIgnoreCase));
            service.Status = serviceStatus;
            Clients.Group("dashboard").SendAsync("ServiceStatusChanged", serviceStatusChanged);
            return Task.CompletedTask;
        }

        public async Task StopService(StopService stopService)
        {
            try
            {
                var group = stopService.AgentId;
                var connectionId = _connectedClients.FirstOrDefault(e => e.Value == group).Key;
                await Clients.Client(connectionId).SendAsync("StopService", stopService);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task StartService(StartService startService)
        {
            var group = startService.AgentId;
            var connectionId = _connectedClients.FirstOrDefault(e => e.Value == group).Key;
            await Clients.Client(connectionId).SendAsync("StartService", startService);
        }
    }
}