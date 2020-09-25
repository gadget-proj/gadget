using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllAgents()
        {
            var keys = _agents.Keys;
            return Ok(keys.Select(k => new { Agent = k }));
        }

        [HttpGet("{agentId:guid}")]
        public async Task<IActionResult> GetAgentInfo(Guid agentId)
        {
            if (_agents.TryGetValue(agentId, out var services))
            {
                return Ok(services);
            }

            return NotFound();
        }

    }
}
