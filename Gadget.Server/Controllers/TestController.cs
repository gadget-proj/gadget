using System;
using System.Threading.Tasks;
using Gadget.Messaging.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IPublishEndpoint _bus;
        private readonly ILogger<TestController> _logger;

        public TestController(IBusControl bus, ILogger<TestController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("publishing");
            await _bus.Publish<IHelloEvent>(new {Content = "Hello my friend", CreatedAt = DateTime.Now});
            _logger.LogInformation("done");
            return Ok();
        }
    }
}