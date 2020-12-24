using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly ISendEndpointProvider _provider;


        public AgentsController(ICollection<Agent> agents, ISendEndpointProvider provider)
        {
            _agents = agents;
            _provider = provider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            var keys = _agents.Select(a => a.Name.Replace("-", ""));
            return await Task.FromResult<IActionResult>(Ok(keys.Select(k => new {Agent = k})));
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
            Console.WriteLine($"setting key {agent}");
            var _bus = await _provider.GetSendEndpoint(new Uri($"exchange:{agent}"));
            await _bus.Send<IStartService>(new { }, context => { context.SetRoutingKey(agent); });
            return Accepted();
        }

        [HttpGet("{agent}/stop")]
        public async Task<IActionResult> StopService(string agent)
        {
            Console.WriteLine($"setting key {agent}");
            var _bus = await _provider.GetSendEndpoint(new Uri($"exchange:{agent}"));
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