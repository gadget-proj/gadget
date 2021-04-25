using System.Threading.Tasks;
using Gadget.Server.Services;
using Gadget.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly ISelectorService _selectorService;

        public ResourceController(ISelectorService selectorService)
        {
            _selectorService = selectorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetResource(string selector)
        {
            var resources = await _selectorService.Match(selector);
            return resources is not null ? Ok(resources) : NotFound();
        }
    }
}