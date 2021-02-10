using Gadget.Messaging.Contracts.Commands;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gadget.Server.Agents.HealthCheck
{
    public class AgentHealthCheck : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AgentHealthCheck(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            using (var scope = _serviceProvider.CreateScope())
            {
                var agentsService = scope.ServiceProvider.GetService<IAgentsService>();
                var client = scope.ServiceProvider.GetService<IRequestClient<CheckAgentHealth>>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    var tmp = await agentsService.GetAgents();
                    var response = await client.GetResponse<CheckAgentHealth>(new { Agent = tmp.First().Name, IsAlive = false },stoppingToken);
                    var mes = response.Message;
                    await Task.Delay(TimeSpan.FromSeconds(6), stoppingToken);
                }
            }
        }
    }
}
