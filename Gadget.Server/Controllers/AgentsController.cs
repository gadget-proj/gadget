using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly ICollection<Agent> _agents;

        public AgentsController(ICollection<Agent> agents)
        {
            _agents = agents;
        }


        [HttpGet]
        [Route("dashboard")]
        public Task<IActionResult> GetAllAgents()
        {
            var keys = _agents.Select(a => a.Name.Replace("-", ""));
            return Task.FromResult<IActionResult>(Ok(keys.Select(k => new {Agent = k})));
        }

        [HttpGet]
        [Route("machine/{agent}")]
        public Task<IActionResult> GetAgentInfo(string agent)
        {
            var services = _agents.FirstOrDefault(a => a.Name.Replace("-", "") == agent)?.Services;
            return services is null
                ? Task.FromResult<IActionResult>(NotFound())
                : Task.FromResult<IActionResult>(Ok(services));
        }
    }
}