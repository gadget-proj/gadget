using Gadget.Server.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gadget.Server.Agents
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly GadgetContext _context;
        private readonly ILogger<AgentsController> _logger;
        private readonly IAgentsService _agentsService;

        public AgentsController(ICollection<Agent> agents, GadgetContext context, ILogger<AgentsController> logger,
            IAgentsService agentsService)
        {
            _context = context;
            _logger = logger;
            _agentsService = agentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            return Ok(await _agentsService.GetAgents());
        }

        [HttpGet("events/{number}")]
        public async Task<IActionResult> GetLatestEvents(int number)
        {
            return Ok(await _agentsService.GetEvents(number));
        }

        [HttpGet("{agent}")]
        public async Task<IActionResult> GetAgentInfo(string agent)
        {
            return Ok(await _agentsService.GetServices(agent));
        }

        [HttpPost("{agent}/{service}/start")]
        public async Task<IActionResult> StartService(string agent, string service)
        {
            await _agentsService.StartService(agent, service);
            return Accepted();
        }

        [HttpPost("{agent}/{service}/stop")]
        public async Task<IActionResult> StopService(string agent, string service)
        {
            await _agentsService.StopService(agent, service);
            return Accepted();
        }
    }
}