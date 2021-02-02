using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.BackgroundServices
{
    public class WebhooksService : BackgroundService
    {
        private readonly ChannelReader<Notification> _channel;
        private readonly HttpClient _client;
        private readonly ILogger<WebhooksService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;


        public WebhooksService(Channel<Notification> channel, HttpClient client, ILogger<WebhooksService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _channel = channel.Reader;
            _client = client;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var notification in _channel.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                await using var context = scope.ServiceProvider.GetRequiredService<NotificationsContext>();
                var agent = await context.Services.FirstOrDefaultAsync(s => s.Agent == notification.Agent,
                    cancellationToken: stoppingToken);
                if (agent is null)
                {
                    _logger.LogInformation($"Could not find agent {notification.Agent}, skipping webhook request");
                    continue;
                }

                _logger.LogInformation(
                    $"Received new webhook notification request for service {notification.ServiceName}");
                await _client.PostAsJsonAsync(notification.Webhook, new InvokeWebhook
                {
                    Content =
                        $"Service : {notification.ServiceName} Agent : {notification.Agent} Status : {notification.Status}"
                }, cancellationToken: stoppingToken);
            }
        }

        public class InvokeWebhook
        {
            [JsonPropertyName("content")] public string Content { get; set; }
        }
    }
}