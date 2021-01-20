using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Server.Agents.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Agents
{
    public interface IAgentsService
    {
        Task<IEnumerable<AgentDto>> GetAgents();
        Task<IEnumerable<ServiceDto>> GetServices(string agentName);
        Task StartService(string agentName, string serviceName);
        Task StopService(string agentName, string serviceName);
    }
}