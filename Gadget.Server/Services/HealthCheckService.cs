// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using Gadget.Messaging.Commands;
// using Gadget.Server.Domain.Entities;
// using Gadget.Server.Hubs;
// using Microsoft.AspNetCore.SignalR;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
//
// namespace Gadget.Server.Services
// {
//     public class HealthCheckService : BackgroundService
//     {
//         private readonly ICollection<Agent> _agents;
//         private readonly IDictionary<string, Guid> _connectedClients;
//         private readonly IHubContext<GadgetHub> _hubContext;
//         private readonly ILogger<HealthCheckService> _logger;
//
//         public HealthCheckService(ILogger<HealthCheckService> logger, IDictionary<string, Guid> connectedClients,
//             IHubContext<GadgetHub> hubContext, ICollection<Agent> agents)
//         {
//             _logger = logger;
//             _connectedClients = connectedClients;
//             _hubContext = hubContext;
//             _agents = agents;
//         }
//
//         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 foreach (var agent in _agents)
//                 {
//                     await _hubContext.Clients.Client(agent.ConnectionId)
//                         .SendAsync("GetServicesReport", new GetAgentHealth(), stoppingToken);
//                     _logger.LogInformation($"Fetching status for client [{agent.Name}]");
//                 }
//
//                 await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
//             }
//         }
//     }
// }