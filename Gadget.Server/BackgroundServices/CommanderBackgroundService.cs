using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Server.Domain.Enums;
using Gadget.Server.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.BackgroundServices
{
    public class CommanderBackgroundService : BackgroundService
    {
        private readonly ILogger<CommanderBackgroundService> _logger;
        private readonly ChannelReader<IServiceStatusChanged> _events;
        private readonly IAgentsService _agentsService;

        public CommanderBackgroundService(ILogger<CommanderBackgroundService> logger,
            Channel<IServiceStatusChanged> events, IServiceProvider services)
        {
            _events = events.Reader;
            _logger = logger;
            _agentsService = services.CreateScope().ServiceProvider.GetService<IAgentsService>();;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var @event in _events.ReadAllAsync(stoppingToken))
            {
                if (Enum.Parse<ServiceStatus>(@event.Status) == ServiceStatus.Stopped)
                {
                    var res = await _agentsService.RestartService(@event.Agent, @event.Name);
                    _logger.LogInformation("Requested service restart {res}", res);
                }

                _logger.LogInformation("New event received {@event}", @event.Status);
            }
        }
    }
}