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
        private readonly ChannelReader<Message> _channel;
        private readonly HttpClient _client;
        private readonly ILogger<WebhooksService> _logger;


        public WebhooksService(Channel<Message> channel, HttpClient client, ILogger<WebhooksService> logger)
        {
            _channel = channel.Reader;
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _channel.ReadAllAsync(stoppingToken))
            {
                await SendWebhookNotification(message, stoppingToken);
            }
        }

        private async Task SendWebhookNotification(Message message, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Sending webhook notification {message.Body}");
            await _client.PostAsJsonAsync(message.Receiver, new InvokeWebhook
            {
                Content = message.Body
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