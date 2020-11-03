using Gadget.Inspector.Models;
using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var cid = Context.ConnectionId;
            _connectedClients.Remove(cid);
            _logger.LogInformation($"Client {cid} has disconnected");
            return Task.CompletedTask;
        }

        public Task RegisterMachineReport(RegisterMachineReport registerMachineReport)
        {
            _logger.LogInformation($"Received new machine report from agent {registerMachineReport.AgentId}");
            _agents[registerMachineReport.AgentId] = registerMachineReport.Services.ToList();
            foreach (var service in registerMachineReport.Services)
            {
                _logger.LogInformation($"Service name : {service.Name} status : {service.Status}");
            }

            return Task.CompletedTask;
        }

        public Task MachineHealthCheck(MachineHealthDataModel model)
        {
            _logger.LogInformation(model.MachineName);
            return Task.CompletedTask;
        }

        //TODO Remove from group on disconnect
        public Task RegisterDashboard(RegisterNewDashboard registerNewDashboard)
        {
            _logger.LogCritical($"HEj hej hej roman");
            var cid = Context.ConnectionId;
            Groups.AddToGroupAsync(cid, "dashboard");
            return Task.CompletedTask;
        }

        public Task Register(RegisterNewAgent registerNewAgent)
        {
            var cid = Context.ConnectionId;
            var agentId = registerNewAgent.AgentId;
            if (_agents.ContainsKey(agentId)) return Task.CompletedTask;
            _agents[agentId] = registerNewAgent.Services.ToList();
            _connectedClients[cid] = agentId;
            return Task.CompletedTask;
        }

        public Task ServiceStatusChanged(ServiceStatusChanged serviceStatusChanged)
        {
            var cid = Context.ConnectionId;
            if (!_connectedClients.TryGetValue(cid, out var agentId))
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