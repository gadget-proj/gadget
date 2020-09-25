using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

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

        public Task Register(RegisterNewAgent registerNewAgent)
        {
            var cid = Context.ConnectionId;
            var agentId = registerNewAgent.AgentId;
            if (_agents.ContainsKey(agentId)) return Task.CompletedTask;
            _agents[agentId] = new List<Service>();
            _connectedClients[cid] = agentId;
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