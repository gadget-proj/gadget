using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.BackgroundServices
{
    /// <summary>
    /// WebhooksService listens on a incoming notifications to emit
    /// </summary>
    public class WebhooksBackgroundService : BackgroundService
    {
        private readonly ChannelReader<DiscordMessage> _channel;
        private readonly ILogger<WebhooksBackgroundService> _logger;
        private readonly IServiceProvider _services;

        public WebhooksBackgroundService(Channel<DiscordMessage> channel, ILogger<WebhooksBackgroundService> logger,
            IServiceProvider services)
        {
            _channel = channel.Reader;
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _channel.ReadAllAsync(stoppingToken))
            {
                var webhooksService = _services.GetService<IWebhooksService>();
                if (webhooksService is null)
                {
                    continue;
                }

                _logger.LogInformation("Processing new webhook request");
                await webhooksService.SendDiscordMessage(message, stoppingToken);
            }
        }
    }
}