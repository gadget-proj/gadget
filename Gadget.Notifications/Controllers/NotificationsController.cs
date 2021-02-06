using System.Threading.Tasks;
using Gadget.Common;
using Gadget.Notifications.Commands;
using Gadget.Notifications.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Notifications.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public NotificationsController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost("{agentName}/{serviceName}")]
        public async Task<IActionResult> CreateNotification(string agentName, string serviceName)
        {
            var command = new CreateNotificationCommand
            {
                AgentName = agentName,
                ServiceName = serviceName,
            };
            await _dispatcher.Send(command);
            return Created("", "");
        }

        [HttpPost("{agentName}/{serviceName}/webhooks")]
        public async Task<IActionResult> CreateWebhook(string agentName, string serviceName,
            CreateWebhook createWebhook)
        {
            var command = new CreateWebhookCommand
            {
                AgentName = agentName,
                ServiceName = serviceName,
                Receiver = createWebhook.Uri
            };
            await _dispatcher.Send(command);
            return Created("", "");
        }
    }
}