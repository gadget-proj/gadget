using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Responses;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Domain.Enums;
using Gadget.Server.Dto.V1;
using Gadget.Server.Dto.V1.Requests;
using Gadget.Server.Persistence;
using Gadget.Server.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Action = Gadget.Server.Domain.Enums.Action;

namespace Gadget.Server.Services
{
    public class AgentsService : IAgentsService
    {
        private readonly GadgetContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AgentsService> _logger;

        public AgentsService(GadgetContext context, IPublishEndpoint publishEndpoint, ILogger<AgentsService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<IEnumerable<AgentDto>> GetAgents()
        {
            var agents = await _context.Agents.ToListAsync();
            return await Task.FromResult(agents.Select(a => new AgentDto(a.Name, a.Address)));
        }

        public async Task<IEnumerable<EventDto>> GetLatestEvents(int count)
        {
            var events = await _context.ServiceEvents
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.Service)
                .ThenInclude(x => x.Agent)
                .Take(count)
                .ToListAsync();

            return await Task.FromResult(events.Select(e =>
                new EventDto(e.Service.Agent.Name, e.Service.Name, e.CreatedAt, e.Status.ToString())));
        }

        public async Task<IEnumerable<EventDto>> GetEvents(string agent, string serviceName, DateTime from, DateTime to,
            int count = int.MaxValue, int skip = 0)
        {
            if (from == DateTime.MinValue)
            {
                from = DateTime.UtcNow.AddYears(-1);
            }

            if (to == DateTime.MinValue)
            {
                to = DateTime.UtcNow;
            }

            var events = await _context.ServiceEvents
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.Service)
                .ThenInclude(x => x.Agent)
                .Where(x => x.Service.Name == serviceName
                    // x.CreatedAt > from &&
                    // x.CreatedAt < to)
                )
                .Skip(skip)
                .Take(count)
                .ToListAsync();

            return await Task.FromResult(events.Select(e =>
                new EventDto(e.Service.Agent.Name, e.Service.Name, e.CreatedAt, e.Status.ToString())));
        }

        public async Task ApplyConfig(IEnumerable<ConfigRequest> requestRules)
        {
            foreach (var (selector, actionRequests) in requestRules)
            {
                var svc = await _context
                    .Services
                    .Include(s => s.Config)
                    .Include(s => s.Agent)
                    .FirstOrDefaultAsync(s => s.Name == selector.ToLower().Trim());
                if (svc is null)
                {
                    continue;
                }

                var config = new Config(actionRequests);
                svc.ApplyConfig(config);
                var requiredAction = svc.RequiredAction();
                if (requiredAction != Action.Pass)
                {
                    switch (requiredAction)
                    {
                        case Action.Stop:
                            await StopService(svc.Agent.Name, svc.Name);
                            break;
                        case Action.Start:
                            await StartService(svc.Agent.Name, svc.Name);
                            break;
                        // ReSharper disable once UnreachableSwitchCaseDueToIntegerAnalysis
                        case Action.Pass:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EventDto>> GetEvents(string agent, string service)
        {
            var ag = await _context.Agents
                .Include(a => a.Services)
                .ThenInclude(s => s.Events)
                .FirstOrDefaultAsync(a => a.Name == agent);

            var svc = ag.Services.FirstOrDefault(s => s.Name == service);
            return svc?.Events.Select(e =>
                new EventDto(e.Service.Agent.Name, e.Service.Name, e.CreatedAt, e.Status.ToString()));
        }

        public async Task<IEnumerable<ServiceDto>> GetServices(string agentName)
        {
            var machine = _context.Agents
                .Include(a => a.Services)
                .ThenInclude(s => s.Events.Take(20))
                .FirstOrDefault(x => x.Name == agentName);
            var services = machine?.Services;
            var dto = services?.Select(s => new ServiceDto(s.Name, s.Status.ToString(), s.LogOnAs, s.Description));
            return await Task.FromResult(dto);
        }

        public async Task<Guid> RestartService(string agent, string serviceName)
        {
            var actionId = Guid.NewGuid();
            var service = $"{agent}/{serviceName}";
            try
            {
                await _publishEndpoint.Publish<IActionResultResponse>(new
                {
                    CorrelationId = actionId,
                    Agent = agent,
                    ServiceName = serviceName
                }, context => context.SetRoutingKey(serviceName));
                var action = new UserAction(Guid.NewGuid(), actionId, DateTime.UtcNow, ActionResult.Accepted, "");
                await _context.UserActions.AddAsync(action);
                _logger.LogInformation($"Requested restart of service {service}", serviceName);
            }
            catch (Exception e)
            {
                var failedAction = new UserAction(Guid.NewGuid(), actionId, DateTime.UtcNow, ActionResult.Failed,
                    e.Message);
                await _context.UserActions.AddAsync(failedAction);
                _logger.LogCritical(e.Message);
            }

            await _context.SaveChangesAsync();
            return actionId;
        }

        public async Task<Guid> StartService(string agentName, string serviceName)
        {
            var actionId = Guid.NewGuid();
            try
            {
                await _publishEndpoint.Publish<IStartService>(
                    new
                    {
                        CorrelationId = actionId,
                        Agent = agentName,
                        ServiceName = serviceName
                    }, context => { context.SetRoutingKey(serviceName); });
                var action = new UserAction(Guid.NewGuid(), actionId, DateTime.UtcNow, ActionResult.Accepted, "");
                await _context.UserActions.AddAsync(action);

                _logger.LogInformation("Requested restart of service {service}", serviceName);
            }
            catch (Exception e)
            {
                var failedAction = new UserAction(Guid.NewGuid(), actionId, DateTime.UtcNow, ActionResult.Failed,
                    e.Message);
                await _context.UserActions.AddAsync(failedAction);
                _logger.LogCritical(e.Message);
            }

            await _context.SaveChangesAsync();
            return actionId;
        }

        public async Task<Guid> StopService(string agentName, string serviceName)
        {
            var actionId = Guid.NewGuid();
            try
            {
                await _publishEndpoint.Publish<IStopService>(new
                    {
                        CorrelationId = actionId,
                        ServiceName = serviceName,
                        Agent = agentName
                    },
                    context => { context.SetRoutingKey(serviceName); });

                var action = new UserAction(Guid.NewGuid(), actionId, DateTime.UtcNow, ActionResult.Accepted, "");
                await _context.UserActions.AddAsync(action);
                _logger.LogInformation("Requested stop of service {service}", serviceName);
            }
            catch (Exception e)
            {
                var failedAction = new UserAction(Guid.NewGuid(), actionId, DateTime.UtcNow, ActionResult.Failed,
                    e.Message);
                await _context.UserActions.AddAsync(failedAction);
                _logger.LogCritical(e.Message);
            }

            await _context.SaveChangesAsync();
            return actionId;
        }
    }
}