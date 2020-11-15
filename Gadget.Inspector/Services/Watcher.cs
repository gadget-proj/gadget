using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Services
{
    public class Watcher : BackgroundService
    {
        private readonly ChannelWriter<ServiceStatusChanged> _channel;
        private readonly IDictionary<string, ServiceControllerStatus> _statuses;
        private readonly ILogger<Watcher> _logger;

        // ReSharper disable once SuggestBaseTypeForParameter
        public Watcher(Channel<ServiceStatusChanged> channel, ILogger<Watcher> logger)
        {
            _logger = logger;
            _channel = channel.Writer;
            _statuses = new Dictionary<string, ServiceControllerStatus>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting Watcher");
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var serviceController in ServiceController.GetServices())
                {
                    if (!_statuses.ContainsKey(serviceController.ServiceName))
                    {
                        _statuses[serviceController.ServiceName] = serviceController.Status;
                        await UpdateStatus(serviceController, stoppingToken);
                    }

                    var previousStatus = _statuses[serviceController.ServiceName];
                    var currentStatus = serviceController.Status;
                    if (currentStatus == previousStatus)
                    {
                        continue;
                    }

                    await UpdateStatus(serviceController, stoppingToken);
                    _statuses[serviceController.ServiceName] = currentStatus;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task UpdateStatus(ServiceController serviceController, CancellationToken stoppingToken)
        {
            await _channel.WaitToWriteAsync(stoppingToken);
            await _channel.WriteAsync(new ServiceStatusChanged
            {
                Name = serviceController.ServiceName,
                Status = serviceController.Status.ToString(),
                AgentId = Environment.MachineName.Replace("-","")
            }, stoppingToken);
        }
    }
}