using Gadget.Hub.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gadget.Hub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IDictionary<string, ICollection<Service>> _agents;

        public AgentsController(IDictionary<string, ICollection<Service>> agents)
        {
            _agents = agents;
        }

        [HttpGet]
        public async Task<IActionResult> Foo()
        {
            if (!_agents.TryGetValue("foo", out var foo))
            {
                foo = new List<Service>();
                _agents["foo"] = foo;
            }
            return Ok(foo);
        }
    }
}
