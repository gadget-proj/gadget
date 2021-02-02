using System;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Persistence;
using Gadget.Notifications.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhooksController : ControllerBase
    {
        private readonly ILogger<WebhooksController> _logger;
        private readonly NotificationsContext _context;

        public WebhooksController(ILogger<WebhooksController> logger, NotificationsContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> TestWebhook()
        {
            _logger.LogInformation("new hook notificiation");
            return await Task.FromResult(Ok());
        }

        [HttpPost("{agentName}/{serviceName}")]
        public async Task<IActionResult> CreateNewWebhook(string agentName, string serviceName,
            CreateWebhook createWebhook)
        {
            var agent = await _context.Services
                .Include(s=>s.Webhooks)
                .FirstOrDefaultAsync(s => s.Agent == agentName && s.Name == serviceName);
            if (agent is null)
            {
                agent = new Service(agentName, serviceName);
                await _context.AddAsync(agent);
            }

            var webhook = new Webhook(new Uri(createWebhook.Uri));
            agent.AddWebhook(webhook);
            await _context.SaveChangesAsync();
            return Created("", "");
        }
    }
}