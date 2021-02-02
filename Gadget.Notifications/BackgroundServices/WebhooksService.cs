using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.BackgroundServices
{
    public class WebhooksService : BackgroundService
    {
        private readonly ChannelReader<Notification> _channel;
        private readonly HttpClient _client;
        private readonly ILogger<WebhooksService> _logger;

        public WebhooksService(Channel<Notification> channel, HttpClient client, ILogger<WebhooksService> logger)
        {
            _channel = channel.Reader;
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var notification in _channel.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation(
                    $"Received new webhook notification request for service {notification.ServiceName}");
                // await _client.PostAsync(notification.Webhook, null!, cancellationToken: stoppingToken);
            }
        }
    }
}