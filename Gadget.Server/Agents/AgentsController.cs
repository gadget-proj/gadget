using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Server.Agents.Dto;
using Gadget.Server.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Server.Agents
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly ICollection<Agent> _agents;
        private readonly IBusControl _busControl;
        private readonly GadgetContext _context;

        public AgentsController(ICollection<Agent> agents, IBusControl busControl, GadgetContext context)
        {
            _agents = agents;
            _busControl = busControl;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            var agents = await _context.Agents
                .ToListAsync();
            var keys = _agents.Select(a => a.Name.Replace("-", ""));
            return await Task.FromResult<IActionResult>(Ok(agents.Select(a => new AgentDto
            {
                Name = a.Name,
                Address = a.Address
            })));
        }

        [HttpGet("{agent}")]
        public Task<IActionResult> GetAgentInfo(string agent)
        {
            var machine = _context.Agents
               .Include(a => a.Services)
               .FirstOrDefault(x=>x.Name == agent);
            var services = machine?.Services;
            return services is null
                ? Task.FromResult<IActionResult>(NotFound())
                : Task.FromResult<IActionResult>(Ok(services.Select(s=>new ServiceDto(s.Name, s.Status, s.LogOnAs, s.Description))));
        }

        [HttpPost("{service}/start")]
        public async Task<IActionResult> StartService(string service)
        {
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"exchange:{service}"));
            await sendEndpoint.Send<IStartService>(new { }, context => { context.SetRoutingKey(service); });
            return Accepted();
        }

        [HttpPost("{service}/stop")]
        public async Task<IActionResult> StopService(string service)
        {
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"exchange:{service}"));
            await sendEndpoint.Send<IStartService>(new { }, context => { context.SetRoutingKey(service); });
            return Accepted();
        }

        [HttpGet("{agent}/{serviceName}")]
        public async Task<IActionResult> GetServiceInfo(string agent, string serviceName)
        {
            var service = _agents.FirstOrDefault(a => a.Name.Replace("-", "") == agent)?.Services
                .FirstOrDefault(s => string.Equals(s.Name, serviceName, StringComparison.CurrentCultureIgnoreCase));
            return await Task.FromResult(Ok(service));
        }
    }
}