using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using Gadget.Server.Domain.Entities;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly ICollection<Agent> _agents;
        private readonly IBusControl _bus;

        public AgentsController(ICollection<Agent> agents, IBusControl bus)
        {
            _agents = agents;
            _bus = bus;
        }

        [HttpGet]
        public Task<IActionResult> GetAllAgents()
        {
            var keys = _agents.Select(a => a.Name.Replace("-", ""));
            return Task.FromResult<IActionResult>(Ok(keys.Select(k => new {Agent = k})));
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
        public async Task<IActionResult> StartService()
        {
            await _bus.Publish<IStartService>(new { });
            return Accepted();
        }

        [HttpGet("{agent}/stop")]
        public async Task<IActionResult> StopService()
        {
            await _bus.Publish<IStopService>(new { });
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