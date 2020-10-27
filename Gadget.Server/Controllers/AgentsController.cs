using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Gadget.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IDictionary<Guid, ICollection<Service>> _agents;

        public AgentsController(IDictionary<Guid, ICollection<Service>> agents)
        {
            _agents = agents;
        }

        [HttpGet]
        public Task<IActionResult> GetAllAgents()
        {
            var keys = _agents.Keys;
            return Task.FromResult<IActionResult>(Ok(keys.Select(k => new {Agent = k})));
        }

        [HttpGet("{agentId:guid}")]
        public Task<IActionResult> GetAgentInfo(Guid agentId)
        {
            return _agents.TryGetValue(agentId, out var services)
                ? Task.FromResult<IActionResult>(Ok(services))
                : Task.FromResult<IActionResult>(NotFound());
        }
    }
}