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
        private readonly IEmailService _emailService;

        public NotificationsController(INotificationsService notificationsService, IEmailService emailService)
        {
            _notificationsService = notificationsService;
            _emailService = emailService;
        }

        [HttpPost("{agentName}/{serviceName}")]
        public async Task<IActionResult> CreateNotification(string agentName, string serviceName,
            CancellationToken cancellationToken)
        {
            await _notificationsService.RegisterNotification(agentName, serviceName, cancellationToken);
            return Created("", "");
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetNotifierTypes()
        {
            var canToken = new CancellationToken();
           await _emailService.SendEmailMessage(new Domain.ValueObjects.EmailMessage("test message", "marcin_mojski@interia.pl"), canToken);
            var types = _notificationsService.GetNotifierTypes();
            return Ok(types);
        }

        [HttpPost("{agentName}/{serviceName}/webhooks")]
        public async Task<IActionResult> CreateWebhook(string agentName, string serviceName,
            CreateWebhook createWebhook, CancellationToken cancellationToken)
        {
            await _notificationsService.RegisterNotifier(agentName, serviceName, createWebhook.Uri,
                NotifierType.Discord, cancellationToken);
            return Created("", "");
        }

        [HttpGet("{agentName}/{serviceName}/webhooks")]
        public async Task<IActionResult> GetWebhooks(string agentName, string serviceName,
            CancellationToken cancellationToken)
        {
            var webhooks = await _notificationsService.GetWebhooks(agentName, serviceName, cancellationToken);
            return Ok(webhooks);
        }
    }
}