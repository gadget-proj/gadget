using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Services
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> _logger;
        private readonly HttpClient _client;

        public WebhooksService(ILogger<WebhooksService> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task SendDiscordMessage(DiscordMessage message, CancellationToken cancellationToken)
        {
            var (body, receiver) = message;
            _logger.LogInformation($"Sending webhook notification {body}");
            await _client.PostAsJsonAsync(receiver, new InvokeWebhook
            {
                Content = body
            }, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Webhook payload
        /// </summary>
        private class InvokeWebhook
        {
            [JsonPropertyName("content")] public string Content { get; set; }
        }
    }
}