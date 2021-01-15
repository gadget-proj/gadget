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
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly ICollection<Agent> _agents;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly GadgetContext _context;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(ICollection<Agent> agents, GadgetContext context, ILogger<AgentsController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _agents = agents;
            _context = context;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
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
                .ThenInclude(s => s.Events)
                .FirstOrDefault(x => x.Name == agent);
            var services = machine?.Services;
            return services is null
                ? Task.FromResult<IActionResult>(NotFound())
                : Task.FromResult<IActionResult>(Ok(services.Select(s =>
                    new ServiceDto(s.Name, s.Status, s.LogOnAs, s.Description, s.Events))));
        }

        [HttpPost("{agent}/{service}/start")]
        public async Task<IActionResult> StartService(string agent, string service)
        {
            await _publishEndpoint.Publish<IStartService>(new
                {
                    ServiceName = service,
                    Agent = agent
                },
                context => { context.SetRoutingKey(service); });
            return Accepted();
        }

        [HttpPost("{agent}/{service}/stop")]
        public async Task<IActionResult> StopService(string agent, string service)
        {
            await _publishEndpoint.Publish<IStopService>(new
                {
                    ServiceName = service,
                    Agent = agent
                },
                context => { context.SetRoutingKey(service); });
            return Accepted();
        }

        [HttpPost("{agent}/{serviceName}")]
        public async Task<IActionResult> GetServiceInfo(string agent, string serviceName)
        {
            var service = _agents.FirstOrDefault(a => a.Name.Replace("-", "") == agent)?.Services
                .FirstOrDefault(s => string.Equals(s.Name, serviceName, StringComparison.CurrentCultureIgnoreCase));
            return await Task.FromResult(Ok(service));
        }
    }
}