using System.Linq;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Domain.Enums;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Persistence;
using Gadget.Notifications.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Notifications.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationsContext _context;

        public NotificationsController(NotificationsContext context)
        {
            _context = context;
        }

        [HttpPost("{agentName}/{serviceName}")]
        public async Task<IActionResult> CreateNotification(string agentName, string serviceName)
        {
            var notification = new Notification(agentName, serviceName);
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return Created("", "");
        }

        [HttpPost("{agentName}/{serviceName}/webhooks")]
        public async Task<IActionResult> CreateWebhook(string agentName, string serviceName,
            CreateWebhook createWebhook)
        {
            var notification = new Notification(agentName, serviceName);
            var notifier = new Notifier(agentName, serviceName, createWebhook.Uri, NotifierType.Discord);
            notification.AddNotifier(notifier);
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return Created("", "");
        }

        [HttpGet("{agentName}/webhooks")]
        public async Task<IActionResult> GetWebhooks(string agentName)
        {
            var webhooks = await _context.Notifications
                .Where(s => s.Agent == agentName)
                .ToListAsync();
            return Ok(webhooks);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(":)");
        }
    }
}