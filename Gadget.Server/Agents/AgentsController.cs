using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using Gadget.Server.Agents.Dto;
using Gadget.Server.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Agents
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly ICollection<Agent> _agents;
        private readonly IBusControl _busControl;

        public AgentsController(ICollection<Agent> agents, IBusControl busControl)
        {
            _agents = agents;
            _busControl = busControl;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            var keys = _agents.Select(a => a.Name.Replace("-", ""));
            return await Task.FromResult<IActionResult>(Ok(_agents.Select(a=>new AgentDto
            {
                Name = a.Name,
                Services = a.Services.Select(s=>new ServiceDto
                {
                    Name = s.Name,
                    Status = s.Status
                })
            })));
        }

        [HttpGet("{agent}")]
        public Task<IActionResult> GetAgentInfo(string agent)
        {
            var services = _agents.FirstOrDefault(a => a.Name.Replace("-", "") == agent)?.Services;
            return services is null
                ? Task.FromResult<IActionResult>(NotFound())
                : Task.FromResult<IActionResult>(Ok(services));
        }

        [HttpGet("{agent}/start")]
        public async Task<IActionResult> StartService(string agent)
        {
            var _bus = await _busControl.GetSendEndpoint(new Uri($"exchange:{agent}"));
            await _bus.Send<IStartService>(new { }, context => { context.SetRoutingKey(agent); });
            return Accepted();
        }

        [HttpGet("{agent}/stop")]
        public async Task<IActionResult> StopService(string agent)
        {
            var _bus = await _busControl.GetSendEndpoint(new Uri($"exchange:{agent}"));
            await _bus.Send<IStartService>(new { }, context => { context.SetRoutingKey(agent); });
            return Accepted();
        }

        [HttpGet("{agent}/{serviceName}")]
        public async Task<IActionResult> GetServiceInfo(string agent, string serviceName)
        {
            var service = _agents.FirstOrDefault(a => a.Name.Replace("-", "") == agent)?.Services
                .FirstOrDefault(s => string.Equals(s.Name, serviceName, StringComparison.CurrentCultureIgnoreCase));
            return Ok(service);
        }
    }
}