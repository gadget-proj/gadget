using System.Threading.Tasks;
using Gadget.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentsService _agentsService;

        public AgentsController(IAgentsService agentsService)
        {
            _agentsService = agentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            return Ok(await _agentsService.GetAgents());
        }

        [HttpGet("events/{count}")]
        public async Task<IActionResult> GetLatestEvents(int count)
        {
            return Ok(await _agentsService.GetLatestEvents(count));
        }

        [HttpGet("{agent}/{service}/events/{count}")]
        public async Task<IActionResult> GetServiceEvents(string agent, string service, int count)
        {
            return Ok(await _agentsService.GetServiceEvents(agent, service, count));
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

        [HttpPost("{agent}/{service}/restart")]
        public async Task<IActionResult> RestartService(string agent, string service)
        {
            await _agentsService.RestartService(agent, service);
            return Accepted();
        }
    }
}