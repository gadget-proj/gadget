using Gadget.Messaging.RegistrationMessages;
using Gadget.Messaging.ServiceMessages;
using Gadget.Server.Models;
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
        private readonly IList<Agent> _agents;
        private readonly ILogger<GadgetHub> _logger;
        

        public GadgetHub(List<Agent> agents, ILogger<GadgetHub> logger)
        {
            _agents = agents;
            _logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var cid = Context.ConnectionId;
            _agents.Remove(_agents.Single(x => x.ConnectionId == cid));
            _logger.LogInformation($"Client {cid} has disconnected");
            return Task.CompletedTask;
        }

        public Task RegisterMachineReport(RegisterMachineReport registerMachineReport)
        {
            _logger.LogInformation($"Received new machine report from agent {registerMachineReport.AgentId}");
            var agent = _agents.FirstOrDefault(x => x.MachineId == registerMachineReport.AgentId);
            if (agent == null)
            {
                _agents.Add(new Agent
                {
                    ConnectionId = Context.ConnectionId,
                    MachineId = registerMachineReport.AgentId,
                    Services = registerMachineReport.Services
                });
            }
            else // jeżeli taka sytuacja nie może mieć miejsca to skasuję ifa, na wszelki wypadek jest
            {
                agent.Services = registerMachineReport.Services.ToList();
                agent.ConnectionId = Context.ConnectionId;
            }

            foreach (var service in registerMachineReport.Services)
            {
                _logger.LogInformation($"Service name : {service.Name} status : {service.Status}");
            }

            return Task.CompletedTask;
        }

        public Task MachineHealthCheck(MachineHealthDataModel model)
        {
            var agent = _agents.FirstOrDefault(x => x.MachineId == model.MachineId);
            if (agent != null) agent.MachineHealthData = model;
            
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

        public Task Register(RegisterNewAgent registerNewAgent) // dodać du klasę MachineHealthDataModel ?
        {
            var cid = Context.ConnectionId;
            var agentId = registerNewAgent.AgentId;
            var agent = _agents.FirstOrDefault(x => x.MachineId == agentId);
            if (agent != null) return Task.CompletedTask;

            _agents.Add(new Agent
            {
                ConnectionId = cid,
                MachineId = agentId,
                Services = registerNewAgent.Services.ToList()
            });
            return Task.CompletedTask;
        }

        public Task ServiceStatusChanged(ServiceStatusChanged serviceStatusChanged)
        {
            var service = GetService(serviceStatusChanged.AgentId, serviceStatusChanged.Name);
            if (service == null) return Task.CompletedTask;

            service.Status = serviceStatusChanged.Status;
            Clients.Group("dashboard").SendAsync("ServiceStatusChanged", serviceStatusChanged);
            return Task.CompletedTask;
        }

        public async Task StopService(StopService stopService)
        {
            var service = GetService(stopService.AgentId, stopService.ServiceName);
            if (service != null)
            {
                var connectionId = _agents.First(x => x.MachineId == stopService.AgentId).ConnectionId;
                await Clients.Client(connectionId).SendAsync("StopService", stopService);
            }
        }

        public async Task StartService(StartService startService)
        {
            var service = GetService(startService.AgentId, startService.ServiceName);
            if (service != null)
            {
                var connectionId = _agents.First(x => x.MachineId == startService.AgentId).ConnectionId;
                await Clients.Client(connectionId).SendAsync("StartService", startService);
            }
        }

        private Service GetService(Guid machineId, string serviceName)
        {
            var agent = _agents.FirstOrDefault(x => x.MachineId == machineId);
            if (agent == null) return null;
            return agent.Services.FirstOrDefault(x =>
                    string.Equals(
                        x.Name, 
                        serviceName, 
                        StringComparison.CurrentCultureIgnoreCase));

        }
    }
}