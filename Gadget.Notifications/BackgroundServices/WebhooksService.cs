using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.BackgroundServices
{
    /// <summary>
    /// WebhooksService listens on a incoming notifications to emit
    /// </summary>
    public class WebhooksService : BackgroundService
    {
        private readonly ChannelReader<DiscordMessage> _channel;
        private readonly HttpClient _client;
        private readonly ILogger<WebhooksService> _logger;


        public WebhooksService(Channel<DiscordMessage> channel, HttpClient client, ILogger<WebhooksService> logger)
        {
            _channel = channel.Reader;
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _channel.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation("Processing new webhook request");
                await SendWebhookNotification(message, stoppingToken);
            }
        }

        private async Task SendWebhookNotification(DiscordMessage discordMessage, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Sending webhook notification {discordMessage.Body}");
            await _client.PostAsJsonAsync(discordMessage.Receiver, new InvokeWebhook
            {
                Content = discordMessage.Body
            }, cancellationToken: stoppingToken);
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