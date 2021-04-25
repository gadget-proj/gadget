using System;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Server.Hubs;
using Gadget.Server.Services;
using Gadget.Server.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gadget.Server.HealthCheck
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
                var bus = scope.ServiceProvider.GetService<IBus>();
                var hub = scope.ServiceProvider.GetService<IHubContext<GadgetHub>>();

                var agents = await agentsService.GetAgents();
                var counter = 0;
                
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (counter ==10) // check new registered agents every tenth check
                    {
                        agents = await agentsService.GetAgents();
                        counter = 0;
                    }

                    foreach (var a in agents)
                    {
                        var client = bus.CreateRequestClient<CheckAgentHealth>(new Uri($"queue:{a.Name}"));
                        var response = client.GetResponse<CheckAgentHealth>(new {IsAlive = false }, stoppingToken);
                        // mass transit documentation says that set request time out aoutside registration wont work
                        if (await Task.WhenAny(response, Task.Delay(2000))== response)
                        {
                           await hub.Clients.Group("dashboard").SendAsync("AgentHealthCheck", new {Agent=a.Name, IsAlive= true });
                        }
                        else
                        {
                            await hub.Clients.Group("dashboard").SendAsync("AgentHealthCheck", new { Agent = a.Name, IsAlive = false });
                        }

                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    }
                    counter++;
                    await Task.Delay(TimeSpan.FromSeconds(6), stoppingToken);
                }
            }
        }
    }
}
