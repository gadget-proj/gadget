using System.Threading;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.Enums;
using Gadget.Notifications.Requests;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Notifications.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        [HttpPost("{agentName}/{serviceName}")]
        public async Task<IActionResult> CreateNotification(string agentName, string serviceName,
            CancellationToken cancellationToken)
        {
            await _notificationsService.RegisterNotification(agentName, serviceName, cancellationToken);
            return Created("", "");
        }

        [HttpPost("{agentName}/{serviceName}/webhooks")]
        public async Task<IActionResult> CreateWebhook(string agentName, string serviceName,
            CreateWebhook createWebhook, CancellationToken cancellationToken)
        {
            await _notificationsService.RegisterNotifier(agentName, serviceName, createWebhook.Uri,
                NotifierType.Discord, cancellationToken);
            return Created("", "");
        }
    }
}